using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Infrastructure.Utils
{
    /// <summary>
    /// Snowflake 风格 ID 生成（64-bit）
    /// layout (from high bit to low):
    /// 1 bit unused | 41 bits timestamp(ms) | 10 bits workerId | 12 bits sequence
    /// </summary>
    internal sealed class SnowflakeIdGenerator
    {
        private readonly object _lock = new();
        private readonly long _epochMilliseconds;
        private readonly int _workerId; // 0..1023
        private const int WorkerIdBits = 10;
        private const int SequenceBits = 12;
        private const long MaxWorkerId = (1L << WorkerIdBits) - 1;
        private const long MaxSequence = (1L << SequenceBits) - 1;

        private long _lastTimestamp = -1L;
        private long _sequence = 0L;

        public SnowflakeIdGenerator(long workerId, DateTime epoch)
        {
            if ( workerId < 0 || workerId > MaxWorkerId ) throw new ArgumentOutOfRangeException(nameof(workerId));
            _workerId = ( int )workerId;
            _epochMilliseconds = new DateTimeOffset(epoch).ToUnixTimeMilliseconds();
        }

        private long CurrentTimeMillis() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        public ulong NextId()
        {
            lock ( _lock )
            {
                var timestamp = CurrentTimeMillis();
                if ( timestamp < _lastTimestamp )
                {
                    // 时钟回拨，等待或抛异常；这里选择等待
                    timestamp = WaitUntil(_lastTimestamp);
                }

                if ( timestamp == _lastTimestamp )
                {
                    _sequence = ( _sequence + 1 ) & MaxSequence;
                    if ( _sequence == 0 )
                    {
                        // sequence 溢出，等待下一毫秒
                        timestamp = WaitUntil(_lastTimestamp);
                    }
                }
                else
                {
                    _sequence = 0;
                }

                _lastTimestamp = timestamp;

                ulong id = (ulong)((((timestamp - _epochMilliseconds) & ((1L << 41) - 1)) << (WorkerIdBits + SequenceBits))
                    | ((_workerId & (int)MaxWorkerId) << SequenceBits)
                    | (_sequence & MaxSequence));

                return id;
            }
        }

        private long WaitUntil(long lastTs)
        {
            long ts;
            do
            {
                Thread.Sleep(1);
                ts = CurrentTimeMillis();
            } while ( ts <= lastTs );

            return ts;
        }

        // 解析方法（用于解码）
        public (DateTime CreatedAtUtc, int WorkerId, long Sequence) Parse(ulong id)
        {
            var timestampPart = (long)((id >> (WorkerIdBits + SequenceBits)) & ((1L << 41) - 1));
            var worker = (int)((id >> SequenceBits) & ((1L << WorkerIdBits) - 1));
            var seq = (long)(id & ((1L << SequenceBits) - 1));
            var millis = timestampPart + _epochMilliseconds;
            return (DateTimeOffset.FromUnixTimeMilliseconds(millis).UtcDateTime, worker, seq);
        }
    }
}
