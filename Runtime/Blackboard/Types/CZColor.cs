using System;
using UnityEngine;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZColor : CZType<Color>
    {
        public CZColor() : base()
        { value = Color.white; }
    }
}
