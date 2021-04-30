using System;
using UnityEngine;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZVector2Int : CZType<Vector2Int>
    {
        public CZVector2Int() : base()
        { Value = Vector2Int.zero; }

        public CZVector2Int(Vector2Int _value) : base(_value) { }

        public static implicit operator CZVector2Int(Vector2Int _other) { return new CZVector2Int(_other); }
    }
}
