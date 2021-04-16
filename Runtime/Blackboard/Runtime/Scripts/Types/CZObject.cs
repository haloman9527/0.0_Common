using System;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZObject : CZType<UnityEngine.Object>
    {
        public CZObject() : base()
        { value = null; }
    }
}
