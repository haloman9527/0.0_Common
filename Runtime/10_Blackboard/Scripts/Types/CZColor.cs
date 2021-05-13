using System;
using UnityEngine;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZColor : CZType<Color>
    {
        public CZColor() : base()
        { Value = Color.white; }

        public CZColor(Color _value) : base(_value) { }

        public static implicit operator CZColor(Color _other) { return new CZColor(_other); }
    }
}
