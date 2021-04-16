using System;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZDouble : CZType<double>
    {
        public CZDouble() : base()
        { value = 0; }
    }
}
