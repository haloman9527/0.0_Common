using System;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZInt : CZType<int>
    {
        public CZInt() : base()
        { value = 0; }
    }
}
