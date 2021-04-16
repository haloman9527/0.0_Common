using System;
using UnityEngine;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZVector2Int : CZType<Vector2Int>
    {
        public CZVector2Int() : base()
        { value = Vector2Int.zero; }
    }
}
