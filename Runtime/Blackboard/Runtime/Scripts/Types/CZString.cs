using System;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZString : CZType<string>
    {
        public CZString() : base()
        { Value = ""; }

        public CZString(string _value) : base(_value) { }

        public static implicit operator CZString(string _other) { return new CZString(_other); }
    }
}
