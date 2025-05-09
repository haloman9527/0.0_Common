using System;

namespace Atom
{
    public interface IBridgedValue<T>
    {
        T Value { get; set; }
    }

    [Serializable]
    public class BridgedValue<T> : IBridgedValue<T>
    {
        private T m_value;

        public T Value
        {
            get => m_value;
            set => m_value = value;
        }

        public BridgedValue(T value)
        {
            this.m_value = value;
        }
    }

    [Serializable]
    public class BridgedValueGetterSetter<T> : IBridgedValue<T>
    {
        private Func<T> m_valueGetter;
        private Action<T> m_valueSetter;

        public T Value
        {
            get => m_valueGetter();
            set => m_valueSetter(value);
        }

        public BridgedValueGetterSetter(Func<T> valueGetter, Action<T> valueSetter)
        {
            this.m_valueGetter = valueGetter;
            this.m_valueSetter = valueSetter;
        }
    }
}