using System;
using System.Collections.Generic;
using UnityEngine;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZGameObjectList : CZType<List<GameObject>>
    {
        public CZGameObjectList() : base()
        { Value = new List<GameObject>(); }

        public CZGameObjectList(List<GameObject> _value) : base(_value) { }

        public static implicit operator CZGameObjectList(List<GameObject> _other) { return new CZGameObjectList(_other); }
    }
}
