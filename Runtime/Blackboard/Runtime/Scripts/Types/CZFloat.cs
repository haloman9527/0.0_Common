using System;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZFloat : CZType<float>
    {
        public CZFloat() : base()
        { value = 0; }
    }
}
