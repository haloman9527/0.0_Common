using System;
using System.Threading;

namespace Atom
{
    public class IdGenerator
    {
        private const int SEQUENCE_BITS = 12;
        private const long SEQUENCE_MASK = -1L ^ (-1L << SEQUENCE_BITS);

        private readonly long _baseTimestamp;
        private long _lastTimestampSequence; // 合并时间戳和序列号

        public IdGenerator(int year, int month, int day)
        {
            _baseTimestamp = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc).Ticks / 10000L;
            _lastTimestampSequence = 0L; // 初始时间戳和序列号均为0
        }

        private long GetCurrentTime()
        {
            return (DateTime.UtcNow.Ticks / 10000L) - _baseTimestamp;
        }

        public long GenerateId()
        {
            int spinCount = 0;
            while (spinCount++ < 1000) // 限制自旋次数防止死循环
            {
                long currentTsSeq = Interlocked.Read(ref _lastTimestampSequence);
                long currentTimestamp = currentTsSeq >> SEQUENCE_BITS;
                long currentSequence = currentTsSeq & SEQUENCE_MASK;

                long actualTimestamp = GetCurrentTime();

                if (actualTimestamp < currentTimestamp)
                {
                    throw new InvalidOperationException("Clock moved backwards.");
                }

                if (actualTimestamp == currentTimestamp)
                {
                    long newSequence = (currentSequence + 1) & SEQUENCE_MASK;
                    if (newSequence != 0)
                    {
                        long newTsSeq = (currentTimestamp << SEQUENCE_BITS) | newSequence;
                        if (Interlocked.CompareExchange(ref _lastTimestampSequence, newTsSeq, currentTsSeq) == currentTsSeq)
                        {
                            return (actualTimestamp << SEQUENCE_BITS) | newSequence;
                        }
                    }
                    else
                    {
                        actualTimestamp = TilNextTimestamp(currentTimestamp);
                    }
                }

                // 时间戳已变化，尝试更新
                long newTsSeqForNewTimestamp = (actualTimestamp << SEQUENCE_BITS) | 0L;
                if (Interlocked.CompareExchange(ref _lastTimestampSequence, newTsSeqForNewTimestamp, currentTsSeq) == currentTsSeq)
                {
                    return (actualTimestamp << SEQUENCE_BITS) | 0L;
                }
            }

            throw new InvalidOperationException("Failed to generate ID after maximum spins.");
        }

        private long TilNextTimestamp(long currentTimestamp)
        {
            long timestamp;
            do
            {
                Thread.SpinWait(10);
                timestamp = GetCurrentTime();
            } while (timestamp <= currentTimestamp);

            return timestamp;
        }
    }
}