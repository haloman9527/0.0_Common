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
using UnityEngine;
using System;

namespace CZToolKit.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true,Inherited = true)]
    public class MinMaxSliderAttribute : UnityEngine.PropertyAttribute {

        public float min, max;
        public float fieldWidth = 50;
        public MinMaxSliderAttribute(float min,float max)
        {
            this.min = min;
            this.max = max; 
        }
    }
}
