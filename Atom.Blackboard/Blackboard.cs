using System;
using System.Collections.Generic;

namespace Atom
{
    public partial class Blackboard<TKey> : IBlackboard<TKey>
    {
        private Dictionary<Type, object> m_Containers = new Dictionary<Type, object>();
        private Dictionary<TKey, object> m_ContainerMap = new Dictionary<TKey, object>();

        public object Get(TKey key)
        {
            if (!this.m_ContainerMap.TryGetValue(key, out var dataContainer))
            {
                return default;
            }

            return ((IDataContainer)dataContainer).Get(key);
        }

        public bool TryGet(TKey key, out object value)
        {
            if (!this.m_ContainerMap.TryGetValue(key, out var dataContainer))
            {
                value = default;
                return false;
            }

            return ((IDataContainer)dataContainer).TryGet(key, out value);
        }

        public bool Contains(TKey key)
        {
            return m_ContainerMap.ContainsKey(key);
        }

        public TValue Get<TValue>(TKey key)
        {
            if (!this.m_ContainerMap.TryGetValue(key, out var dataContainer))
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
            if (!this.m_ContainerMap.TryGetValue(key, out var dataContainer))
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
            if (!m_ContainerMap.TryGetValue(key, out var dataContainer))
            {
                if (isValueType)
                {
                    if (!m_Containers.TryGetValue(type, out dataContainer))
                    {
                        m_Containers[type] = dataContainer = new DataContainer<TValue>();
                    }
                }
                else
                {
                    if (!m_Containers.TryGetValue(typeof(object), out dataContainer))
                    {
                        m_Containers[type] = dataContainer = new DataContainer<object>();
                    }
                }

                m_ContainerMap[key] = dataContainer;
            }

            if (isValueType)
                return ((IDataContainer<TValue>)dataContainer).Set(key, value);
            else
                return ((IDataContainer<object>)dataContainer).Set(key, value);
        }

        public bool Remove(TKey key)
        {
            if (!m_ContainerMap.TryGetValue(key, out var dataContainer))
                return false;

            m_ContainerMap.Remove(key);
            ((IDataContainer)dataContainer).Remove(key);
            return true;
        }

        public void Clear()
        {
            foreach (var pair in m_Containers)
            {
                var dataContainer = pair.Value;
                ((IDataContainer)dataContainer).Clear();
            }

            m_Containers.Clear();
            m_ContainerMap.Clear();
        }

        public IEnumerable<KeyValuePair<TKey, object>> EnumerateValues()
        {
            foreach (var dataContainer in m_Containers.Values)
            {
                foreach (var pair in ((IDataContainer)dataContainer).Values())
                {
                    yield return pair;
                }
            }
        }
    }
}