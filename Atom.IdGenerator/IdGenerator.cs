using System;
using System.Threading;

public class IdGenerator
{
    private const int SEQUENCE_BITS = 12;
    private const long SEQUENCE_MASK = -1L ^ (-1L << SEQUENCE_BITS);

    private long baseTimestamp;
    private long lastTimestamp;
    private long sequence;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="year"> 基准年 </param>
    /// <param name="month"> 基准月 </param>
    /// <param name="day"> 基准日 </param>
    public IdGenerator(int year, int month, int day)
    {
        this.baseTimestamp = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc).Ticks / 10000L;
        this.lastTimestamp = this.baseTimestamp;
        this.sequence = 0;
    }

    private long GetCurrentTime()
    {
        return (DateTime.UtcNow.Ticks / 10000L) - this.baseTimestamp;
    }

    public long GenerateId()
    {
        lock (this)
        {
            var timestamp = this.GetCurrentTime();
            if (lastTimestamp == timestamp)
            {
                sequence = Interlocked.Add(ref this.sequence, 1) & SEQUENCE_MASK;
                if (sequence == 0)
                {
                    // 在锁内获取当前时间并等待下一个时间戳
                    Thread.Sleep(1);
                    timestamp += 1;
                }
            }
            else
            {
                sequence = 0L;
            }

            lastTimestamp = timestamp;
            return (timestamp << SEQUENCE_BITS) | sequence;
        }
    }

    /// <summary>
    /// 等待下个时间戳. 
    /// </summary>
    /// <returns></returns>
    private long TilNextTimestamp(long currentTimeStamp)
    {
        var timestamp = GetCurrentTime();

        while (timestamp <= currentTimeStamp)
        {
            timestamp = GetCurrentTime();
        }

        return timestamp;
    }
}