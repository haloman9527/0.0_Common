using System;
using UnityEngine;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZVector3Int : CZType<Vector3Int>
    {
        public CZVector3Int() : base()
        { value = Vector3Int.zero; }
    }
}
