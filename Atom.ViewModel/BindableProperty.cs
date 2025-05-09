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
 *  Github: https://github.com/haloman9527
 *  Blog: https://www.haloman.net/
 *
 */

#endregion

using System;
using System.Collections.Generic;

namespace Atom
{
    [Serializable]
    public class BindableProperty<T> : IBindableProperty<T>, IBindableProperty
    {
        private IBridgedValue<T> bridgedValue;

        public event Action<T, T> ValueChanged;
        public event Action<object, object> BoxedValueChanged;

        public T Value
        {
            get => bridgedValue.Value;
            set => SetValue(value);
        }

        public object BoxedValue
        {
            get => Value;
            set => Value = (T)value;
        }

        public Type ValueType => TypeCache<T>.TYPE;
        
        public BindableProperty() : this(default(T))
        {
        }

        public BindableProperty(T value)
        {
            this.bridgedValue = new BridgedValue<T>(value);
        }

        public BindableProperty(Func<T> getter, Action<T> setter)
        {
            this.bridgedValue = new BridgedValueGetterSetter<T>(getter, setter);
        }

        public BindableProperty(IBridgedValue<T> bridgedValue)
        {
            this.bridgedValue = bridgedValue;
        }

        private void NotifyValueChanged_Internal(T oldValue, T newValue)
        {
            ValueChanged?.Invoke(oldValue, newValue);
            BoxedValueChanged?.Invoke(oldValue, newValue);
        }

        public IBindableProperty<TOut> AsBindableProperty<TOut>()
        {
            return this as BindableProperty<TOut>;
        }

        public void RegisterValueChangedEvent(Action<T, T> onValueChanged)
        {
            this.ValueChanged += onValueChanged;
        }

        public void UnregisterValueChangedEvent(Action<T, T> onValueChanged)
        {
            this.ValueChanged -= onValueChanged;
        }

        public bool SetValue(T value)
        {
            if (ValidEquals(Value, value))
                return false;
            var oldValue = Value;
            bridgedValue.Value = value;
            NotifyValueChanged_Internal(oldValue, value);
            return true;
        }

        public void SetValueWithoutNotify(T value)
        {
            bridgedValue.Value = value;
        }

        public bool SetValue(object value)
        {
            return SetValue((T)value);
        }

        public void SetValueWithoutNotify(object value)
        {
            bridgedValue.Value = (T)value;
        }

        public void ClearValueChangedEvent()
        {
            while (this.ValueChanged != null)
            {
                this.ValueChanged -= this.ValueChanged;
            }

            while (this.BoxedValueChanged != null)
            {
                this.BoxedValueChanged -= this.BoxedValueChanged;
            }
        }

        public void NotifyValueChanged()
        {
            NotifyValueChanged_Internal(Value, Value);
        }

        public override string ToString()
        {
            return (Value != null ? Value.ToString() : "null");
        }

        protected virtual bool ValidEquals(T oldValue, T newValue)
        {
            return EqualityComparer<T>.Default.Equals(oldValue, newValue);
        }
    }
}