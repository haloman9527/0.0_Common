#region 注 释

/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 半只龙虾人
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */

#endregion

using System;

namespace CZToolKit
{
    /// <summary>
    /// 分布式ID自增算法 Snowflake 雪花算法
    /// </summary>
    public class Snowflake64
    {
        #region Const

        // 前41位就可以使用69年, 10位的可支持1023台机器, 最后12位序列号可以在1毫秒内产生4095个自增的ID

        /// <summary>
        /// 默认基准时间戳(2019-01-01 00:00:00)
        /// </summary>
        private const long DEFAULT_BASE_TIMESTAMP = 1546272000000L;

        /// <summary>
        /// 机器码字节数, 4个字节用来保存机器码(定义为Long类型会出现, 最大偏移64位, 所以左移64位没有意义)
        /// </summary>
        private const int WORKER_ID_BITS = 5;

        /// <summary>
        /// 数据字节数
        /// </summary>
        private const int DATACENTER_ID_BITS = 5;

        /// <summary>
        /// 计数器字节数, 计数器字节数, 12个字节用来保存计数码 
        /// </summary>
        private const int SEQUENCE_BITS = 12;

        /// <summary>
        /// 机器码数据左移位数, 就是后面计数器占用的位数(12). 
        /// </summary>
        private const int WORKER_ID_SHIFT = SEQUENCE_BITS;

        /// <summary>
        /// 数据ID左移位数(17)
        /// </summary>
        private const int DATACENTER_ID_SHIFT = SEQUENCE_BITS + WORKER_ID_BITS;

        /// <summary>
        /// 时间戳左移动位数就是机器码+计数器总字节数+数据字节数(22). 
        /// </summary>
        private const int TIMESTAMP_LEFT_SHIFT = SEQUENCE_BITS + WORKER_ID_BITS + DATACENTER_ID_BITS;

        /// <summary>
        /// 一微秒内可以产生计数, 如果达到该值(4096)则等到下一微妙在进行生成. 
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

        /// <summary>
        /// 基准时间戳, 小于当前时间即可, 一旦确定不能变动. 
        /// 分布式项目请保持此时间戳一致. 
        /// </summary>
        private readonly long baseTimestamp;

        /// <summary>
        /// 毫秒计数器. 
        /// </summary>
        private long sequence;

        /// <summary>
        /// 最后一次的时间戳. 
        /// </summary>
        private long lastTimestamp = DEFAULT_BASE_TIMESTAMP;

        /// <summary>
        /// 10位的数据机器位中的高位(机器码). 
        /// </summary>
        public long WorkerID { get; protected set; }

        /// <summary>
        /// 10位的数据机器位中的低位(数据ID). 
        /// </summary>
        public long DatacenterID { get; protected set; }

        /// <summary>
        /// 线程锁对象. 
        /// </summary>
        private readonly object @lock = new object();

        public long CurrentID { get; private set; }

        /// <summary>
        /// 基于Twitter的snowflake算法. 
        /// </summary>
        /// <param name="workerID"> 10位的数据机器位中的低位, 默认不应该超过5位(31) </param>
        /// <param name="datacenterID"> 10位的数据机器位中的高位, 默认不应该超过5位(31) </param>
        public Snowflake64(byte workerID, byte datacenterID) : this(workerID, datacenterID, DEFAULT_BASE_TIMESTAMP)
        {
        }

        /// <summary>
        /// 基于Twitter的snowflake算法. 
        /// </summary>
        /// <param name="workerID"> 10位的数据机器位中的低位, 默认不应该超过5位(31) </param>
        /// <param name="datacenterID"> 10位的数据机器位中的高位, 默认不应该超过5位(31) </param>
        /// <param name="baseTimestamp">  </param>
        public Snowflake64(byte workerID, byte datacenterID, long baseTimestamp)
        {
            this.baseTimestamp = baseTimestamp;
            this.WorkerID = workerID;
            this.DatacenterID = datacenterID;

            if (workerID > MAX_WORKER_ID)
            {
                throw new ArgumentException($"worker Id can't be greater than {MAX_WORKER_ID} or less than 0");
            }

            if (datacenterID > MAX_DATACENTER_ID)
            {
                throw new ArgumentException($"datacenter Id can't be greater than {MAX_DATACENTER_ID} or less than 0");
            }
        }

        /// <summary>
        /// 获取下一个ID, 该方法线程安全. 
        /// </summary>
        /// <returns></returns>
        public long NextID()
        {
            lock (@lock)
            {
                var timestamp = DateTime.Now.ToFileTimeUtc();
                if (timestamp < lastTimestamp)
                {
                    throw new Exception(
                        $"Clock moved backwards or wrapped around. Refusing to generate id for {lastTimestamp - timestamp} ticks");
                }

                if (lastTimestamp == timestamp)
                {
                    // 同一微秒中生成ID
                    // 用&运算计算该微秒内产生的计数是否已经到达上限
                    sequence = (sequence + 1) & SEQUENCE_MASK;
                    // 一微妙内产生的ID计数已达上限, 等待下一微妙
                    if (sequence == 0)
                        timestamp = TilNextMillis();
                }
                else
                    sequence = 0L;

                lastTimestamp = timestamp; //把当前时间戳保存为最后生成ID的时间戳
                CurrentID = ((timestamp - baseTimestamp) << TIMESTAMP_LEFT_SHIFT) |
                            (DatacenterID << DATACENTER_ID_SHIFT) |
                            (WorkerID << WORKER_ID_SHIFT) | sequence;

                return CurrentID;
            }
        }

        /// <summary>
        /// 获取时间截. 
        /// </summary>
        /// <returns></returns>
        private long TilNextMillis()
        {
            var timestamp = DateTime.Now.ToFileTimeUtc();
            while (timestamp <= lastTimestamp)
            {
                timestamp = DateTime.Now.ToFileTimeUtc();
            }

            return timestamp;
        }
    }

    public class Snowflake32
    {
        private long lastTimestamp;
        private int sequence;

        private readonly object @lock = new object();

        public int NextID()
        {
            lock (@lock)
            {
                var timestamp = DateTime.Now.ToFileTimeUtc();
                if (lastTimestamp == timestamp)
                {
                    if (sequence <= 15)
                    {
                        sequence++;
                    }
                    else
                    {
                        timestamp = TilNextMillis();
                        sequence = 0;
                    }
                }
                else
                    sequence = 0;

                lastTimestamp = timestamp;

                return (int)((timestamp) & (long.MaxValue >> 36)) | (sequence << 28);
            }
        }

        private long TilNextMillis()
        {
            var timestamp = DateTime.Now.ToFileTimeUtc();
            while (timestamp <= lastTimestamp)
            {
                timestamp = DateTime.Now.ToFileTimeUtc();
            }

            return timestamp;
        }
    }

    public static class Util_Snowflake
    {
        private static readonly Snowflake64 s_Snowflake = new Snowflake64(0, 0);
        private static readonly Snowflake32 s_Snowflake32 = new Snowflake32();

        /// <summary>
        /// 使用雪花算法生成不重复的ID. 
        /// </summary>
        /// <returns></returns>
        public static long GenerateIDBySnowflake()
        {
            return s_Snowflake.NextID();
        }
        
        public static int GenerateIDBySnowflake32()
        {
            return s_Snowflake32.NextID();
        }
    }
}