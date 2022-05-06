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

namespace CZToolKit.Core.ViewModel
{
    [Serializable]
    public class BindableProperty<T> : IBindableProperty, IBindableProperty<T>
    {
        public event Func<T> Getter;
        public event Action<T> Setter;
        public event Action<T> onValueChanged;
        public event Action<object> onBoxedValueChanged;

        public T Value
        {
            get
            {
                if (Getter == null)
                    throw new NotImplementedException("haven't get method");
                return Getter();
            }
            set
            {
                if (Setter == null)
                    throw new NotImplementedException("haven't set method");
                if (Equals(Value, value))
                    return;
                Setter(value);
                NotifyValueChanged();
            }
        }
        public object ValueBoxed
        {
            get { return Value; }
            set { Value = (T)value; }
        }
        public Type ValueType { get { return typeof(T); } }
        public BindableProperty() { }
        public BindableProperty(Func<T> getter) { this.Getter = getter; }
        public BindableProperty(Func<T> getter, Action<T> setter) { this.Getter = getter; this.Setter = setter; }

        public void SetGetterSetter(Func<T> getter, Action<T> setter)
        {
            this.Getter = getter;
            this.Setter = setter;
        }
        public IBindableProperty<T1> AsBindableProperty<T1>()
        {
            return this as BindableProperty<T1>;
        }
        public void RegisterValueChangedEvent(Action<T> onValueChanged)
        {
            this.onValueChanged += onValueChanged;
        }
        public void RegisterValueChangedEvent<T1>(Action<T1> onValueChanged)
        {
            AsBindableProperty<T1>().RegisterValueChangedEvent(onValueChanged);
        }
        public void UnregisterValueChangedEvent(Action<T> onValueChanged)
        {
            this.onValueChanged -= onValueChanged;
        }
        public void UnregisterValueChangedEvent<T1>(Action<T1> onValueChanged)
        {
            AsBindableProperty<T1>().UnregisterValueChangedEvent(onValueChanged);
        }
        public void SetValueWithNotify(T value)
        {
            Setter?.Invoke(value);
            NotifyValueChanged();
        }
        public void SetValueWithoutNotify(T value)
        {
            Setter?.Invoke(value);
        }
        public void SetValueWithNotify(object value)
        {
            SetValueWithoutNotify((T)value);
            NotifyValueChanged();
        }
        public void SetValueWithoutNotify(object value)
        {
            SetValueWithoutNotify((T)value);
        }
        public void ClearValueChagnedEvent()
        {
            while (this.onValueChanged != null)
                this.onValueChanged -= this.onValueChanged;
        }
        public override string ToString()
        {
            return (Value != null ? Value.ToString() : "null");
        }

        public void NotifyValueChanged()
        {
            onValueChanged?.Invoke(Value);
            onBoxedValueChanged?.Invoke(Value);
        }
    }
}
