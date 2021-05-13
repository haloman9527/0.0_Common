using System;
using UnityEngine;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZBounds : CZType<Bounds>
    {
        public CZBounds() : base()
        { Value = new Bounds(); }

        public CZBounds(Bounds _value) : base(_value) { }

        public static implicit operator CZBounds(Bounds _other) { return new CZBounds(_other); }
    }
}
