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
        private T m_Value;

        public T Value
        {
            get => m_Value;
            set => m_Value = value;
        }

        public BridgedValue(T value)
        {
            this.m_Value = value;
        }
    }

    [Serializable]
    public class BridgedValueGetterSetter<T> : IBridgedValue<T>
    {
        private Func<T> m_ValueGetter;
        private Action<T> m_ValueSetter;

        public T Value
        {
            get => m_ValueGetter();
            set => m_ValueSetter(value);
        }

        public BridgedValueGetterSetter(Func<T> valueGetter, Action<T> valueSetter)
        {
            this.m_ValueGetter = valueGetter;
            this.m_ValueSetter = valueSetter;
        }
    }
}