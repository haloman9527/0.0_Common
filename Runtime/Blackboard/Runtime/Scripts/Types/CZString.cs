using System;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZString : CZType<string>
    {
        public CZString() : base()
        { value = ""; }
    }
}
