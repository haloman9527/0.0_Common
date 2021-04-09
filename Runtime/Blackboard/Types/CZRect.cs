using System;
using UnityEngine;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZRect : CZType<Rect>
    {
        public CZRect() : base()
        { value = new Rect(); }
    }
}
