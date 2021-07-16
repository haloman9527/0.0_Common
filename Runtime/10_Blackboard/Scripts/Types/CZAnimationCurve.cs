#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 半只龙虾人
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */
#endregion
using System;
using UnityEngine;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZAnimationCurve : CZType<AnimationCurve>
    {
        public CZAnimationCurve() : base()
        { Value = new AnimationCurve(); }

        public CZAnimationCurve(AnimationCurve _value) : base(_value) { }

        public static implicit operator CZAnimationCurve(AnimationCurve _other) { return new CZAnimationCurve(_other); }
    }
}
