using System;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
#if ODIN_INSPECTOR
    [HideLabel]
    [HideReferenceObjectPicker]
#endif
    public class CZType<T> : ICZType
    {
#if UNITY_EDITOR
        [UnityEngine.SerializeField]
#endif
#if ODIN_INSPECTOR
        [HideLabel]
        [HideReferenceObjectPicker]
#endif
        protected T value;
        public Type PropertyType => typeof(T);
        public T Value { get { return value; } set { this.value = value; } }

        public CZType() { }
        public CZType(T _value) { value = _value; }
    }
}