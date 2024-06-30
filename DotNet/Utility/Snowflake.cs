using System;

namespace CZToolKit
{
    /// <summary>
    /// 分布式ID自增算法 Snowflake 雪花算法
    /// </summary>
    public class Snowflake
    {
        #region Const

        /// <summary>
        /// 机器ID位数
        /// </summary>
        private const int WORKER_ID_BITS = 5;

        /// <summary>
        /// 数据中心ID位数
        /// </summary>
        private const int DATACENTER_ID_BITS = 5;

        /// <summary>
        /// 计数器位数
        /// </summary>
        private const int SEQUENCE_BITS = 12;

        /// <summary>
        /// 机器码数据左移位数, 就是后面计数器占用的位数
        /// </summary>
        private const int WORKER_ID_SHIFT = SEQUENCE_BITS;

        /// <summary>
        /// 数据ID左移位数
        /// </summary>
        private const int DATACENTER_ID_SHIFT = SEQUENCE_BITS + WORKER_ID_BITS;

        /// <summary>
        /// 时间戳左移动位数就是机器码+计数器总位数+数据位数
        /// </summary>
        private const int TIMESTAMP_LEFT_SHIFT = SEQUENCE_BITS + WORKER_ID_BITS + DATACENTER_ID_BITS;

        /// <summary>
        /// 一单位时间内可以产生计数, 如果达到该值(4096)则等到下一单位时间在进行生成. 
        /// </summary>
        private const long SEQUENCE_MASK = -1L ^ (-1L << SEQUENCE_BITS);

        /// <summary>
        /// 最大机器ID(5位:0-31). 
        /// </summary>
        private const long MAX_WORKER_ID = -1L ^ (-1L << WORKER_ID_BITS);

        /// <summary>
        /// 最大数据ID(5位:0-31). 
        /// </summary>
        private const long MAX_DATACENTER_ID = -1L ^ (-1L << DATACENTER_ID_BITS);

        #endregion

        public interface ITimeProvider
        {
            /// <summary>
            /// 获取当前时间
            /// </summary>
            /// <returns></returns>
            long GetCurrentTime();
        }

        /// <summary>
        /// 基准时间戳, 小于当前时间即可, 一旦确定不能变动. 
        /// 分布式项目请保持此时间戳一致. 
        /// </summary>
        private readonly long epoch;

        /// <summary>
        /// 最后一次的时间戳. 
        /// </summary>
        private long lastTimestamp;

        /// <summary>
        /// 单位时间内计数器. 
        /// </summary>
        private long sequence;

        /// <summary>
        /// 10位的数据机器位中的高位(机器码). 
        /// </summary>
        public long WorkerID { get; protected set; }

        /// <summary>
        /// 10位的数据机器位中的低位(数据ID). 
        /// </summary>
        public long DatacenterID { get; protected set; }

        public long LastID { get; private set; }

        public ITimeProvider TimeProvider { get; }

        /// <summary>
        /// 基于Twitter的snowflake算法. 
        /// </summary>
        /// <param name="workerID"> 10位的数据机器位中的低位, 默认不应该超过31(5位) </param>
        /// <param name="datacenterID"> 10位的数据机器位中的高位, 默认不应该超过31(5位) </param>
        /// <param name="epoch"> 基准时间戳 </param>
        /// <param name="timeProvider"> 时间提供者 </param>
        public Snowflake(byte workerID, byte datacenterID, long epoch, ITimeProvider timeProvider)
        {
            this.epoch = epoch;
            this.lastTimestamp = epoch;
            this.WorkerID = workerID;
            this.DatacenterID = datacenterID;
            this.TimeProvider = timeProvider;

            if (WorkerID > MAX_WORKER_ID)
                throw new ArgumentException($"worker Id can't be greater than {MAX_WORKER_ID} or less than 0");

            if (DatacenterID > MAX_DATACENTER_ID)
                throw new ArgumentException($"datacenter Id can't be greater than {MAX_DATACENTER_ID} or less than 0");
        }

        /// <summary>
        /// 基于Twitter的snowflake算法. 
        /// </summary>
        /// <param name="workerID"> 不超过1024 </param>
        /// <param name="epoch"> 基准时间戳 </param>
        /// <param name="timeProvider"> 时间提供者 </param>
        public Snowflake(int workerID, long epoch, ITimeProvider timeProvider) : this((byte)(workerID & 32), (byte)((workerID >> 5) & 32), epoch, timeProvider)
        {
        }

        /// <summary>
        /// 获取下一个ID, 该方法线程安全. 
        /// </summary>
        /// <returns></returns>
        public long NextID()
        {
            lock (this)
            {
                var timestamp = GetCurrentTimestamp();
                if (timestamp < lastTimestamp)
                {
                    throw new Exception(
                        $"Clock moved backwards or wrapped around. Refusing to generate id for {lastTimestamp - timestamp} ticks");
                }

                if (lastTimestamp == timestamp)
                {
                    // 同一单位时间中生成ID
                    // 用&运算计算该单位时间内产生的计数是否已经到达上限
                    sequence = (sequence + 1) & SEQUENCE_MASK;
                    // 一单位时间内产生的ID计数已达上限, 等待下一单位时间
                    if (sequence == 0)
                        timestamp = TilNextTimestamp();
                }
                else
                    sequence = 0L;

                // 把当前时间戳保存为最后生成ID的时间戳
                lastTimestamp = timestamp;
                LastID = ((timestamp - epoch) << TIMESTAMP_LEFT_SHIFT) |
                         (DatacenterID << DATACENTER_ID_SHIFT) |
                         (WorkerID << WORKER_ID_SHIFT) | sequence;

                return LastID;
            }
        }

        /// <summary>
        /// 获取当前时间戳. 
        /// </summary>
        /// <returns></returns>
        private long GetCurrentTimestamp()
        {
            return TimeProvider.GetCurrentTime();
        }

        /// <summary>
        /// 等待下个时间戳. 
        /// </summary>
        /// <returns></returns>
        private long TilNextTimestamp()
        {
            var timestamp = GetCurrentTimestamp();
            while (timestamp <= lastTimestamp)
            {
                timestamp = GetCurrentTimestamp();
            }

            return timestamp;
        }
    }
}