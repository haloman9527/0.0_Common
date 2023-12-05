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
 *  Blog: https://www.mindgear.net/
 *
 */
#endregion
using System;
using UnityEngine;

namespace CZToolKit.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class ProgressBarAttribute : UnityEngine.PropertyAttribute
    {
        public readonly float min;
        public readonly float max;
        public readonly bool drawMinMaxValue;
        public float height = 20;

        public ProgressBarAttribute(float _min, float _max, bool _drawMinMaxValue = false)
        {
            min = _min;
            max = _max;
            drawMinMaxValue = _drawMinMaxValue;
        }
    }
}
