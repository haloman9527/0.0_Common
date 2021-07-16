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

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace CZToolKit.Core.Blackboards
{
#if ODIN_INSPECTOR
    [HideReferenceObjectPicker]
#endif
    public interface ICZType
    {
        Type ValueType { get; }

        object GetValue();
        void SetValue(object _value);
    }

    [Serializable]
    public class CZType<T> : ICZType
    {
        [SerializeField]
        T value;

        public T Value { get { return value; } set { this.value = value; } }

        public Type ValueType
        {
            get
            {
                if (value == null) return typeof(T);
                return value.GetType();
            }
        } 

        public CZType() { }

        public CZType(T _value) { value = _value; }

        public object GetValue() { return value; }

        public void SetValue(object _value) { value = (T)_value; }


        public static implicit operator CZType<T>(T _other) { return new CZType<T>(_other); }

    }
}