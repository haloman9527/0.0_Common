using System.Collections.Generic;

namespace Atom
{
    public partial class Blackboard<TKey>
    {
        private interface IDataContainer
        {
            object Get(TKey key);

            bool TryGet(TKey key, out object value);

            bool Set(TKey key, object value);

            bool Remove(TKey key);

            void Clear();

            IEnumerable<KeyValuePair<TKey, object>> Values();
        }

        private interface IDataContainer<TValue>
        {
            TValue Get(TKey key);

            bool TryGet(TKey key, out TValue value);

            bool Set(TKey key, TValue value);

            bool Remove(TKey key);

            void Clear();

            IEnumerable<KeyValuePair<TKey, TValue>> Values();
        }

        private class DataContainer<TValue> : IDataContainer, IDataContainer<TValue>
        {
            private Dictionary<TKey, TValue> m_Data = new Dictionary<TKey, TValue>();

            object IDataContainer.Get(TKey key)
            {
                if (this.m_Data.TryGetValue(key, out var value))
                {
                    return value;
                }
                return null;
            }

            public TValue Get(TKey key)
            {
                if (this.m_Data.TryGetValue(key, out var value))
                {
                    return value;
                }
                return default;
            }

            bool IDataContainer.TryGet(TKey key, out object value)
            {
                if (this.m_Data.TryGetValue(key, out var v))
                {
                    value = v;
                    return true;
                }
                value = null;
                return false;
            }

            public bool TryGet(TKey key, out TValue value)
            {
                return this.m_Data.TryGetValue(key, out value);
            }

            bool IDataContainer.Set(TKey key, object value)
            {
                var tmpValue = (TValue)value;
                if (this.m_Data.TryGetValue(key, out var v) && v.GetHashCode() == tmpValue.GetHashCode())
                {
                    return false;
                }
                this.m_Data[key] = tmpValue;
                return true;
            }

            public bool Set(TKey key, TValue value)
            {
                if (this.m_Data.TryGetValue(key, out var v) && v.GetHashCode() == value.GetHashCode())
                {
                    return false;
                }
                this.m_Data[key] = value;
                return true;
            }

            IEnumerable<KeyValuePair<TKey, object>> IDataContainer.Values()
            {
                foreach (var pair in m_Data)
                {
                    yield return new KeyValuePair<TKey, object>(pair.Key, pair.Value);
                }
            }

            public IEnumerable<KeyValuePair<TKey, TValue>> Values()
            {
                foreach (var pair in m_Data)
                {
                    yield return new KeyValuePair<TKey, TValue>(pair.Key, pair.Value);
                }
            }

            public bool Remove(TKey key)
            {
                return m_Data.Remove(key);
            }

            public void Clear()
            {
                m_Data.Clear();
            }
        }
    }
}