using System;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZObject : CZType<UnityEngine.Object>
    {
        public CZObject() : base()
        { Value = null; }

        public CZObject(UnityEngine.Object _value) : base(_value) { }

        public static implicit operator CZObject(UnityEngine.Object _other) { return new CZObject(_other); }
    }
}
