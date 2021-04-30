using System;
using System.Collections.Generic;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZObjectList : CZType<List<UnityEngine.Object>>
    {
        public CZObjectList() : base()
        { Value = null; }

        public CZObjectList(List<UnityEngine.Object> _value) : base(_value) { }

        public static implicit operator CZObjectList(List<UnityEngine.Object> _other) { return new CZObjectList(_other); }
    }
}
