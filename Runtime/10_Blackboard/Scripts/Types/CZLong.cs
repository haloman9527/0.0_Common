using System;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZLong : CZType<long>
    {
        public CZLong() : base()
        { Value = 0; }

        public CZLong(long _value) : base(_value) { }

        public static implicit operator CZLong(long _other) { return new CZLong(_other); }
    }
}
