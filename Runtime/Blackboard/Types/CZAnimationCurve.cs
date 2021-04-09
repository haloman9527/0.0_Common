using System;
using UnityEngine;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZAnimationCurve : CZType<AnimationCurve>
    {
        public CZAnimationCurve() : base()
        { value = new AnimationCurve(); }
    }
}
