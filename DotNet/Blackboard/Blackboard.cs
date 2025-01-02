using System;
using System.Collections.Generic;

namespace Moyo.Blackboard
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

        private interface IDataContainer<T>
        {
            T Get(TKey key);

            bool TryGet(TKey key, out T value);

            bool Set(TKey key, T value);

            bool Remove(TKey key);

            void Clear();

            IEnumerable<KeyValuePair<TKey, T>> Values();
        }

        private class DataContainer<T> : IDataContainer, IDataContainer<T>
        {
            private Dictionary<TKey, T> data = new Dictionary<TKey, T>();

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
                var tmpValue = (T)value;
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

            public T Get(TKey key)
            {
                if (this.data.TryGetValue(key, out var value))
                {
                    return value;
                }

                return default;
            }

            public bool TryGet(TKey key, out T value)
            {
                return this.data.TryGetValue(key, out value);
            }

            public bool Set(TKey key, T value)
            {
                if (this.data.TryGetValue(key, out var v) && v.GetHashCode() == value.GetHashCode())
                {
                    return false;
                }

                this.data[key] = value;
                return true;
            }

            public IEnumerable<KeyValuePair<TKey, T>> Values()
            {
                foreach (var pair in data)
                {
                    yield return new KeyValuePair<TKey, T>(pair.Key, pair.Value);
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

        private Dictionary<Type, IDataContainer> containers = new Dictionary<Type, IDataContainer>();
        private Dictionary<TKey, IDataContainer> containerMap = new Dictionary<TKey, IDataContainer>();

        public object Get(TKey key)
        {
            if (!this.containerMap.TryGetValue(key, out var dataContainer))
            {
                return default;
            }

            return dataContainer.Get(key);
        }

        public bool TryGet(TKey key, out object value)
        {
            if (!this.containerMap.TryGetValue(key, out var dataContainer))
            {
                value = default;
                return false;
            }

            return dataContainer.TryGet(key, out value);
        }

        public bool Contains(TKey key)
        {
            return containerMap.ContainsKey(key);
        }

        public T Get<T>(TKey key)
        {
            if (!this.containerMap.TryGetValue(key, out var dataContainer))
            {
                return default;
            }

            var type = TypeCache<T>.TYPE;
            var isValueType = type.IsValueType;
            if (isValueType)
            {
                return ((DataContainer<T>)dataContainer).Get(key);
            }

            return (T)((DataContainer<object>)dataContainer).Get(key);
        }

        public bool TryGet<T>(TKey key, out T value)
        {
            if (!this.containerMap.TryGetValue(key, out var dataContainer))
            {
                value = default;
                return false;
            }

            var type = TypeCache<T>.TYPE;
            var isValueType = type.IsValueType;
            if (isValueType)
            {
                return ((DataContainer<T>)dataContainer).TryGet(key, out value);
            }

            var result = ((DataContainer<object>)dataContainer).TryGet(key, out var v);
            value = (T)v;
            return result;
        }

        public bool Set<T>(TKey key, T value)
        {
            var type = TypeCache<T>.TYPE;
            var isValueType = type.IsValueType;
            if (!containerMap.TryGetValue(key, out var dataContainer))
            {
                if (isValueType)
                {
                    if (!containers.TryGetValue(type, out dataContainer))
                    {
                        containers[type] = dataContainer = new DataContainer<T>();
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
                return ((IDataContainer<T>)dataContainer).Set(key, value);
            else
                return ((IDataContainer<object>)dataContainer).Set(key, value);
        }

        public bool Remove(TKey key)
        {
            if (!containerMap.TryGetValue(key, out var dataContainer))
                return false;

            containerMap.Remove(key);
            dataContainer.Remove(key);
            return true;
        }

        public void Clear()
        {
            foreach (var pair in containers)
            {
                pair.Value.Clear();
            }

            containers.Clear();
            containerMap.Clear();
        }

        public IEnumerable<KeyValuePair<TKey, object>> EnumerateValues()
        {
            foreach (var container in containers.Values)
            {
                foreach (var pair in container.Values())
                {
                    yield return pair;
                }
            }
        }
    }
}