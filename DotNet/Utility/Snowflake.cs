using System;

namespace CZToolKit
{
    /// <summary>
    /// Snowflake 雪花算法
    /// </summary>
    public partial class Snowflake
    {
        #region Const

        /// <summary>
        /// 机器Id位数
        /// </summary>
        private const int WORKER_ID_BITS = 5;

        /// <summary>
        /// 数据中心Id位数
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
        /// 数据Id左移位数
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
        /// 最大机器Id(5位:0-31). 
        /// </summary>
        private const long MAX_WORKER_ID = -1L ^ (-1L << WORKER_ID_BITS);

        /// <summary>
        /// 最大数据Id(5位:0-31). 
        /// </summary>
        private const long MAX_DATACENTER_ID = -1L ^ (-1L << DATACENTER_ID_BITS);

        #endregion

        /// <summary>
        /// 最后一次生成Id的时间戳. 
        /// </summary>
        private long lastTimestamp;

        /// <summary>
        /// 单位时间内生成Id计数器. 
        /// </summary>
        private long sequence;

        /// <summary>
        /// 10位的数据机器位中的高位(机器码). 
        /// </summary>
        private long WorkerId { get; set; }

        /// <summary>
        /// 10位的数据机器位中的低位(数据Id). 
        /// </summary>
        private long DatacenterId { get; set; }

        /// <summary>
        /// 时间戳提供者，用以适配不同的时间戳规则
        /// </summary>
        private ITimeProvider TimeProvider { get; set; }

        /// <summary>
        /// 最后一次生成Id的时间戳. 
        /// </summary>
        private long LastTimestamp => lastTimestamp;

        /// <summary>
        /// 单位时间内生成Id计数器. 
        /// </summary>
        private long Sequence => sequence;

        /// <summary>
        /// snowflake算法
        /// </summary>
        /// <param name="workerId"> 10位的数据机器位中的低位, 默认不应该超过31(5位) </param>
        /// <param name="dataCenterId"> 10位的数据机器位中的高位, 默认不应该超过31(5位) </param>
        /// <param name="timeProvider"> 时间戳提供者，用以适配不同的时间戳规则 </param>
        public Snowflake(byte workerId, byte dataCenterId, ITimeProvider timeProvider)
        {
            Init(workerId, dataCenterId, timeProvider);
        }

        /// <summary>
        /// snowflake算法
        /// </summary>
        /// <param name="workerId"> 不超过1024 </param>
        /// <param name="timeProvider"> 时间戳提供者，用以适配不同的时间戳规则 </param>
        public Snowflake(int workerId, ITimeProvider timeProvider)
        {
            Init((byte)(workerId & 32), (byte)((workerId >> 5) & 32), timeProvider);
        }

        private void Init(byte workerId, byte datacenterId, ITimeProvider timeProvider)
        {
            this.lastTimestamp = timeProvider.GetCurrentTime();
            this.sequence = 0;
            this.WorkerId = workerId;
            this.DatacenterId = datacenterId;
            this.TimeProvider = timeProvider;

            if (WorkerId > MAX_WORKER_ID)
                throw new ArgumentException($"worker Id can't be greater than {MAX_WORKER_ID} or less than 0");

            if (DatacenterId > MAX_DATACENTER_ID)
                throw new ArgumentException($"datacenter Id can't be greater than {MAX_DATACENTER_ID} or less than 0");
        }

        /// <summary>
        /// 等待下个时间戳. 
        /// </summary>
        /// <returns></returns>
        private long TilNextTimestamp()
        {
            var timestamp = TimeProvider.GetCurrentTime();
            
            while (timestamp <= lastTimestamp)
            {
                timestamp = TimeProvider.GetCurrentTime();
            }

            return timestamp;
        }

        /// <summary>
        /// 获取下一个Id, 该方法线程安全. 
        /// </summary>
        /// <returns></returns>
        public long NextId()
        {
            lock (this)
            {
                var timestamp = TimeProvider.GetCurrentTime();
                
                if (timestamp < lastTimestamp)
                {
                    throw new Exception(
                        $"Clock moved backwards or wrapped around. Refusing to generate id for {lastTimestamp - timestamp} ticks");
                }

                if (lastTimestamp == timestamp)
                {
                    // 同一单位时间中生成Id
                    // 用&运算计算该单位时间内产生的计数是否已经到达上限
                    sequence = (sequence + 1) & SEQUENCE_MASK;
                    // 一单位时间内产生的Id计数已达上限, 等待下一单位时间
                    if (sequence == 0)
                        timestamp = TilNextTimestamp();
                }
                else
                    sequence = 0L;

                // 把当前时间戳保存为最后生成Id的时间戳
                lastTimestamp = timestamp;
                return ((timestamp) << TIMESTAMP_LEFT_SHIFT) | (DatacenterId << DATACENTER_ID_SHIFT) | (WorkerId << WORKER_ID_SHIFT) | sequence;
            }
        }
    }

    public partial class Snowflake
    {
        public interface ITimeProvider
        {
            /// <summary> 获取当前时间 </summary>
            /// <returns></returns>
            long GetCurrentTime();
        }

        public class UtcMSDateTimeProvider : ITimeProvider
        {
            private readonly long epoch;

            public UtcMSDateTimeProvider(DateTime utcTime)
            {
                GC.Collect();
                epoch = utcTime.Ticks / 10000;
            }

            public long GetCurrentTime()
            {
                return DateTime.UtcNow.Ticks / 10000 - epoch;
            }
        }
    }

    public partial class Snowflake
    {
        public static Snowflake BaseUTC2020 = new Snowflake(0, new UtcMSDateTimeProvider(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)));
    }
}