using System;
using UnityEngine;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZVector2 : CZType<Vector2>
    {
        public CZVector2() : base()
        { value = Vector2.zero; }
    }
}
