using System;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZChar : CZType<char>
    {
        public CZChar() : base()
        { Value = ' '; }

        public CZChar(char _value) : base(_value) { }

        public static implicit operator CZChar(char _other) { return new CZChar(_other); }
    }
}
