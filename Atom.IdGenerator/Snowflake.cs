using System;
using System.Threading;

namespace Atom
{
    /// <summary>
    /// Snowflake 雪花算法 - 性能优化版本
    /// </summary>
    public partial class Snowflake
    {
        #region Define

        public interface ITimeProvider
        {
            /// <summary> 获取当前时间 </summary>
            /// <returns></returns>
            long GetCurrentTime();
        }

        #endregion

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
        private const int DATA_CENTER_ID_SHIFT = SEQUENCE_BITS + WORKER_ID_BITS;

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

        /// <summary>
        /// 预分配的异常消息，避免字符串分配
        /// </summary>
        private static readonly string CLOCK_BACKWARDS_MESSAGE = "Clock moved backwards. Refusing to generate id for {0} ticks";

        #endregion

        /// <summary>
        /// 最后一次生成Id的时间戳. 
        /// </summary>
        private long m_LastTimestamp;

        /// <summary>
        /// 单位时间内生成Id计数器. 
        /// </summary>
        private long m_LastSequence;

        /// <summary>
        /// 10位的数据机器位中的高位(机器码). 
        /// </summary>
        private readonly long m_WorkerId;

        /// <summary>
        /// 10位的数据机器位中的低位(数据Id). 
        /// </summary>
        private readonly long m_DataCenterId;

        /// <summary>
        /// 预计算的组合ID，减少位运算
        /// </summary>
        private readonly long m_CombinedId;

        /// <summary>
        /// 时间戳提供者，用以适配不同的时间戳规则
        /// </summary>
        private readonly ITimeProvider m_TimeProvider;

        /// <summary>
        /// 自旋锁，替代对象锁
        /// </summary>
        private SpinLock m_SpinLock = new SpinLock();

        /// <summary>
        /// snowflake算法
        /// </summary>
        /// <param name="workerId">10位的数据机器位中的低位, 默认不应该超过31(5位)</param>
        /// <param name="dataCenterId">10位的数据机器位中的高位, 默认不应该超过31(5位)</param>
        /// <param name="timeProvider">时间戳提供者，用以适配不同的时间戳规则</param>
        /// <param name="lastTimestamp">最后一次生成id的时间</param>
        /// <param name="lastSequence">最后一次生成id时的seq</param>
        /// <exception cref="ArgumentException"></exception>
        public Snowflake(byte workerId, byte dataCenterId, ITimeProvider timeProvider, long lastTimestamp, int lastSequence)
        {
            if (workerId > MAX_WORKER_ID)
                throw new ArgumentException($"worker Id can't be greater than {MAX_WORKER_ID} or less than 0");

            if (dataCenterId > MAX_DATACENTER_ID)
                throw new ArgumentException($"datacenter Id can't be greater than {MAX_DATACENTER_ID} or less than 0");

            this.m_WorkerId = workerId;
            this.m_DataCenterId = dataCenterId;
            this.m_CombinedId = (dataCenterId << DATA_CENTER_ID_SHIFT) | (workerId << WORKER_ID_SHIFT);
            this.m_TimeProvider = timeProvider;
            this.m_LastTimestamp = lastTimestamp;
            this.m_LastSequence = lastSequence;
        }

        /// <summary>
        /// snowflake算法
        /// </summary>
        /// <param name="workerId">10位的数据机器位中的低位, 默认不应该超过31(5位)</param>
        /// <param name="dataCenterId">10位的数据机器位中的高位, 默认不应该超过31(5位)</param>
        /// <param name="timeProvider">时间戳提供者，用以适配不同的时间戳规则</param>
        public Snowflake(byte workerId, byte dataCenterId, ITimeProvider timeProvider)
            : this(workerId, dataCenterId, timeProvider, timeProvider.GetCurrentTime(), -1)
        {
        }

        /// <summary>
        /// snowflake算法
        /// </summary>
        /// <param name="workerId">不超过1023</param>
        /// <param name="timeProvider">时间戳提供者，用以适配不同的时间戳规则 </param>
        public Snowflake(int workerId, ITimeProvider timeProvider)
            : this((byte)(workerId & 31), (byte)((workerId >> 5) & 31), timeProvider, timeProvider.GetCurrentTime(), -1)
        {
        }

        public long LastTimestamp
        {
            get { return m_LastTimestamp; }
        }

        public long LastSequence
        {
            get { return m_LastSequence; }
        }
        
        /// <summary>
        /// 等待下个时间戳
        /// </summary>
        /// <returns></returns>
        private long TilNextTimestamp()
        {
            var timestamp = m_TimeProvider.GetCurrentTime();
            var spinWait = new SpinWait();

            while (timestamp <= m_LastTimestamp)
            {
                spinWait.SpinOnce();
                timestamp = m_TimeProvider.GetCurrentTime();
            }

            return timestamp;
        }

        /// <summary>
        /// 获取下一个Id, 该方法线程安全
        /// </summary>
        /// <returns></returns>
        public long GenerateId()
        {
            var lockTaken = false;
            try
            {
                m_SpinLock.Enter(ref lockTaken);
                return GenerateIdInternal();
            }
            finally
            {
                if (lockTaken)
                    m_SpinLock.Exit();
            }
        }

        /// <summary>
        /// 内部ID生成逻辑，已加锁保护
        /// </summary>
        /// <returns></returns>
        private long GenerateIdInternal()
        {
            var timestamp = m_TimeProvider.GetCurrentTime();

            if (timestamp < m_LastTimestamp)
            {
                throw new Exception(string.Format(CLOCK_BACKWARDS_MESSAGE, m_LastTimestamp - timestamp));
            }

            if (m_LastTimestamp == timestamp)
            {
                // 同一单位时间中生成Id
                // 用&运算计算该单位时间内产生的计数是否已经到达上限
                m_LastSequence = (m_LastSequence + 1) & SEQUENCE_MASK;
                // 一单位时间内产生的Id计数已达上限, 等待下一单位时间
                if (m_LastSequence == 0)
                    timestamp = TilNextTimestamp();
            }
            else
            {
                m_LastSequence = 0L;
            }

            // 把当前时间戳保存为最后生成Id的时间戳
            m_LastTimestamp = timestamp;

            // 使用预计算的组合ID，减少位运算
            return (timestamp << TIMESTAMP_LEFT_SHIFT) | m_CombinedId | m_LastSequence;
        }
    }

    public partial class Snowflake
    {
        public static readonly Snowflake BaseUtc2020 = new Snowflake(0, new UtcMSDateTimeProvider(2020, 1, 1));

        /// <summary>
        /// 原始时间提供者，保持向后兼容
        /// </summary>
        public class UtcMSDateTimeProvider : ITimeProvider
        {
            private readonly long m_Epoch;

            public UtcMSDateTimeProvider(int year, int month, int day)
            {
                m_Epoch = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc).Ticks / 10000;
            }

            public long GetCurrentTime()
            {
                return DateTime.UtcNow.Ticks / 10000 - m_Epoch;
            }
        }
    }
}