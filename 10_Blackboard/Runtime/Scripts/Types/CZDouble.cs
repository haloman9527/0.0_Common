using System;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZDouble : CZType<double>
    {
        public CZDouble() : base()
        { Value = 0; }

        public CZDouble(double _value) : base(_value) { }

        public static implicit operator CZDouble(double _other) { return new CZDouble(_other); }
    }
}
