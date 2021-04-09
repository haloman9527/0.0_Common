using System;
using UnityEngine;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZGradient : CZType<Gradient>
    {
        public CZGradient() : base()
        { value = new Gradient(); }
    }
}
