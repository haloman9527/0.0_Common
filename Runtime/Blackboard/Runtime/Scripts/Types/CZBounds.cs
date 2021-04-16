using System;
using UnityEngine;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZBounds : CZType<Bounds>
    {
        public CZBounds() : base()
        { value = new Bounds(); }
    }
}
