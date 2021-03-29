using UnityEngine;
using System;

namespace CZToolKit.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true,Inherited = true)]
    public class MinMaxSliderAttribute : PropertyAttribute {

        public float min, max;
        public float fieldWidth = 50;
        public MinMaxSliderAttribute(float min,float max)
        {
            this.min = min;
            this.max = max; 
        }
    }
}
