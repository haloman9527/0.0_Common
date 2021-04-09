using System;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZLong : CZType<long>
    {
        public CZLong() : base()
        { value = 0; }
    }
}
