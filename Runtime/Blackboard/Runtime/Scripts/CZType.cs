using System;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace CZToolKit.Core.Blackboards
{
#if ODIN_INSPECTOR
    [HideLabel]
    [HideReferenceObjectPicker]
#endif
    public interface ICZType
    {
        Type PropertyType { get; }

    }

    [Serializable]
    public class CZType<T> : ICZType
    {
#if ODIN_INSPECTOR
        [HideLabel]
        [HideReferenceObjectPicker]
        [SuffixLabel("@PropertyType.Name")]
#endif
        [SerializeField]
        protected T value;
        public Type PropertyType => typeof(T);
        public T Value { get { return value; } set { this.value = value; } }

        public CZType() { }
        public CZType(T _value) { value = _value; }
    }
}