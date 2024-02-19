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

namespace CZToolKit
{
    [Serializable]
    public class BindableProperty<T> : IBindableProperty<T>
    {
        private event Func<T> Getter;
        private event Action<T> Setter;
        
        public event ValueChangedEvent<T> onValueChanged;
        public event ValueChangedEvent<object> onBoxedValueChanged;

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
                if (ValidEquals(Value, value))
                    return;
                var oldValue = Value;
                Setter(value);
                NotifyValueChanged_Internal(oldValue, value);
            }
        }

        public object ValueBoxed
        {
            get { return Value; }
            set { Value = (T)value; }
        }

        public Type ValueType
        {
            get { return typeof(T); }
        }
        
        public BindableProperty(Func<T> getter, Action<T> setter)
        {
            InitValue_Insternal(getter, setter);
        }
        
        private void InitValue_Insternal(Func<T> getter, Action<T> setter)
        {
            this.Getter = getter;
            this.Setter = setter;
        }

        private void NotifyValueChanged_Internal(T oldValue, T newValue)
        {
            onValueChanged?.Invoke(oldValue, newValue);
            onBoxedValueChanged?.Invoke(oldValue, newValue);
        }

        public IBindableProperty<TOut> AsBindableProperty<TOut>()
        {
            return this as BindableProperty<TOut>;
        }

        public void RegisterValueChangedEvent(ValueChangedEvent<T> onValueChanged)
        {
            this.onValueChanged += onValueChanged;
        }

        public void UnregisterValueChangedEvent(ValueChangedEvent<T> onValueChanged)
        {
            this.onValueChanged -= onValueChanged;
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

        public void ClearValueChangedEvent()
        {
            while (this.onValueChanged != null)
                this.onValueChanged -= this.onValueChanged;
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
            return Equals(oldValue, newValue);
        }
    }
}