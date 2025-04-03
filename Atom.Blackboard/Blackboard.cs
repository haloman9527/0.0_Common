using System;
using System.Collections.Generic;

namespace Atom
{
    public class Blackboard<TKey> : IBlackboard<TKey>
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
            private Dictionary<TKey, TValue> data = new Dictionary<TKey, TValue>();

            object IDataContainer.Get(TKey key)
            {
                if (this.data.TryGetValue(key, out var value))
                    return value;
                return null;
            }

            bool IDataContainer.TryGet(TKey key, out object value)
            {
                if (this.data.TryGetValue(key, out var v))
                {
                    value = v;
                    return true;
                }

                value = default;
                return false;
            }

            bool IDataContainer.Set(TKey key, object value)
            {
                var tmpValue = (TValue)value;
                if (this.data.TryGetValue(key, out var v) && v.GetHashCode() == tmpValue.GetHashCode())
                {
                    return false;
                }

                this.data[key] = tmpValue;
                return true;
            }

            IEnumerable<KeyValuePair<TKey, object>> IDataContainer.Values()
            {
                foreach (var pair in data)
                {
                    yield return new KeyValuePair<TKey, object>(pair.Key, pair.Value);
                }
            }

            public TValue Get(TKey key)
            {
                if (this.data.TryGetValue(key, out var value))
                {
                    return value;
                }

                return default;
            }

            public bool TryGet(TKey key, out TValue value)
            {
                return this.data.TryGetValue(key, out value);
            }

            public bool Set(TKey key, TValue value)
            {
                if (this.data.TryGetValue(key, out var v) && v.GetHashCode() == value.GetHashCode())
                {
                    return false;
                }

                this.data[key] = value;
                return true;
            }

            public IEnumerable<KeyValuePair<TKey, TValue>> Values()
            {
                foreach (var pair in data)
                {
                    yield return new KeyValuePair<TKey, TValue>(pair.Key, pair.Value);
                }
            }

            public bool Remove(TKey key)
            {
                return data.Remove(key);
            }

            public void Clear()
            {
                data.Clear();
            }
        }

        private Dictionary<Type, object> containers = new Dictionary<Type, object>();
        private Dictionary<TKey, object> containerMap = new Dictionary<TKey, object>();

        public object Get(TKey key)
        {
            if (!this.containerMap.TryGetValue(key, out var dataContainer))
            {
                return default;
            }

            return ((IDataContainer)dataContainer).Get(key);
        }

        public bool TryGet(TKey key, out object value)
        {
            if (!this.containerMap.TryGetValue(key, out var dataContainer))
            {
                value = default;
                return false;
            }

            return ((IDataContainer)dataContainer).TryGet(key, out value);
        }

        public bool Contains(TKey key)
        {
            return containerMap.ContainsKey(key);
        }

        public TValue Get<TValue>(TKey key)
        {
            if (!this.containerMap.TryGetValue(key, out var dataContainer))
            {
                return default;
            }

            var type = TypeCache<TValue>.TYPE;
            var isValueType = type.IsValueType;
            if (isValueType)
            {
                return ((DataContainer<TValue>)dataContainer).Get(key);
            }

            return (TValue)((DataContainer<object>)dataContainer).Get(key);
        }

        public bool TryGet<TValue>(TKey key, out TValue value)
        {
            if (!this.containerMap.TryGetValue(key, out var dataContainer))
            {
                value = default;
                return false;
            }

            var type = TypeCache<TValue>.TYPE;
            var isValueType = type.IsValueType;
            if (isValueType)
            {
                return ((DataContainer<TValue>)dataContainer).TryGet(key, out value);
            }

            var result = ((DataContainer<object>)dataContainer).TryGet(key, out var v);
            value = (TValue)v;
            return result;
        }

        public bool Set<TValue>(TKey key, TValue value)
        {
            var type = TypeCache<TValue>.TYPE;
            var isValueType = type.IsValueType;
            if (!containerMap.TryGetValue(key, out var dataContainer))
            {
                if (isValueType)
                {
                    if (!containers.TryGetValue(type, out dataContainer))
                    {
                        containers[type] = dataContainer = new DataContainer<TValue>();
                    }
                }
                else
                {
                    if (!containers.TryGetValue(typeof(object), out dataContainer))
                    {
                        containers[type] = dataContainer = new DataContainer<object>();
                    }
                }

                containerMap[key] = dataContainer;
            }

            if (isValueType)
                return ((IDataContainer<TValue>)dataContainer).Set(key, value);
            else
                return ((IDataContainer<object>)dataContainer).Set(key, value);
        }

        public bool Remove(TKey key)
        {
            if (!containerMap.TryGetValue(key, out var dataContainer))
                return false;

            containerMap.Remove(key);
            ((IDataContainer)dataContainer).Remove(key);
            return true;
        }

        public void Clear()
        {
            foreach (var pair in containers)
            {
                var dataContainer = pair.Value;
                ((IDataContainer)dataContainer).Clear();
            }

            containers.Clear();
            containerMap.Clear();
        }

        public IEnumerable<KeyValuePair<TKey, object>> EnumerateValues()
        {
            foreach (var dataContainer in containers.Values)
            {
                foreach (var pair in ((IDataContainer)dataContainer).Values())
                {
                    yield return pair;
                }
            }
        }
    }
}