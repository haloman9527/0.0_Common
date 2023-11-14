using System;

namespace CZToolKit
{
    /// <summary>
    /// 分布式ID自增算法 Snowflake 雪花算法
    /// </summary>
    public class Snowflake
    {
        #region Const
        // 前41位就可以使用69年, 10位的可支持1023台机器, 最后12位序列号可以在1毫秒内产生4095个自增的ID

        /// <summary>
        /// 毫秒, 默认基准时间戳(2020-01-01 00:00:00)
        /// </summary>
        public const long DEFAULT_BASE_TIMESTAMP = 1577836800000L;

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
        /// 一毫秒内可以产生计数, 如果达到该值(4096)则等到下一毫秒在进行生成. 
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
        
        #region Static
        private static readonly Snowflake s_Snowflake = new Snowflake(0, 0);

        /// <summary>
        /// 使用默认配置生成ID.
        /// </summary>
        /// <returns></returns>
        public static long GenerateID()
        {
            return s_Snowflake.NextID();
        }
        #endregion

        /// <summary>
        /// 基准时间戳(毫秒), 小于当前时间即可, 一旦确定不能变动. 
        /// 分布式项目请保持此时间戳一致. 
        /// </summary>
        private readonly long baseTimestamp;

        /// <summary>
        /// 最后一次的时间戳(毫秒). 
        /// </summary>
        private long lastTimestamp;

        /// <summary>
        /// 毫秒内计数器. 
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

        /// <summary>
        /// 基于Twitter的snowflake算法. 
        /// </summary>
        /// <param name="workerID"> 10位的数据机器位中的低位, 默认不应该超过5位(31) </param>
        /// <param name="datacenterID"> 10位的数据机器位中的高位, 默认不应该超过5位(31) </param>
        /// <param name="baseTimestamp"> 基准时间戳(GMT时间) </param>
        public Snowflake(byte workerID, byte datacenterID, long baseTimestamp = DEFAULT_BASE_TIMESTAMP)
        {
            this.baseTimestamp = baseTimestamp;
            this.lastTimestamp = baseTimestamp;
            this.WorkerID = workerID;
            this.DatacenterID = datacenterID;

            if (WorkerID > MAX_WORKER_ID)
                throw new ArgumentException($"worker Id can't be greater than {MAX_WORKER_ID} or less than 0");

            if (DatacenterID > MAX_DATACENTER_ID)
                throw new ArgumentException($"datacenter Id can't be greater than {MAX_DATACENTER_ID} or less than 0");
        }

        /// <summary>
        /// 基于Twitter的snowflake算法. 
        /// </summary>
        /// <param name="workerID"> 10位ID, 默认不应该超过10位(1024) </param>
        /// <param name="baseTimestamp"> 基准时间戳(GMT时间) </param>
        public Snowflake(uint workerID, long baseTimestamp = DEFAULT_BASE_TIMESTAMP)
        {
            this.baseTimestamp = baseTimestamp;
            this.lastTimestamp = baseTimestamp;
            this.WorkerID = workerID & 32;
            this.DatacenterID = (workerID >> 5) & 32;

            if (WorkerID > MAX_WORKER_ID)
                throw new ArgumentException($"worker Id can't be greater than {MAX_WORKER_ID} or less than 0");

            if (DatacenterID > MAX_DATACENTER_ID)
                throw new ArgumentException($"datacenter Id can't be greater than {MAX_DATACENTER_ID} or less than 0");
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
                    // 同一毫秒中生成ID
                    // 用&运算计算该毫秒内产生的计数是否已经到达上限
                    sequence = (sequence + 1) & SEQUENCE_MASK;
                    // 一毫秒内产生的ID计数已达上限, 等待下一毫秒
                    if (sequence == 0)
                        timestamp = TilNextTimestamp();
                }
                else
                    sequence = 0L;
                
                // 把当前时间戳保存为最后生成ID的时间戳
                lastTimestamp = timestamp; 
                LastID = ((timestamp - baseTimestamp) << TIMESTAMP_LEFT_SHIFT) |
                         (DatacenterID << DATACENTER_ID_SHIFT) |
                         (WorkerID << WORKER_ID_SHIFT) | sequence;

                return LastID;
            }
        }
        
        /// <summary>
        /// 获取当提前时间戳(毫秒). 
        /// </summary>
        /// <returns></returns>
        private long GetCurrentTimestamp()
        {
            return DateTime.Now.ToFileTimeUtc() / 10000 - 11644473600000L;
        }

        /// <summary>
        /// 等待下个时间戳(毫秒). 
        /// </summary>
        /// <returns></returns>
        private long TilNextTimestamp()
        {
            var timestamp =GetCurrentTimestamp();
            while (timestamp <= lastTimestamp)
            {
                timestamp = GetCurrentTimestamp();
            }

            return timestamp;
        }
    }
}