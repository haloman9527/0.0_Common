using System;
using UnityEngine;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZGameObject : CZType<GameObject>
    {
        public CZGameObject() : base()
        { Value = null; }

        public CZGameObject(GameObject _value) : base(_value) { }

        public static implicit operator CZGameObject(GameObject _other) { return new CZGameObject(_other); }
    }
}
