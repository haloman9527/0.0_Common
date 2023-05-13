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
using System.Collections.Generic;

namespace CZToolKit.Common.Blackboard
{
    public enum NotifyType
    {
        Added,
        Changed,
        Remove
    }

    public class Blackboard<TKey>
    {
        public interface IDataContainer
        {
            object Get(TKey key);

            bool TryGet(TKey key, out object value);

            void Remove(TKey key);

            void Clear();
        }

        public class DataContainer<T> : IDataContainer
        {
            private Dictionary<TKey, T> data = new Dictionary<TKey, T>();

            public T this[TKey key]
            {
                get { return Get(key); }
                set { Set(key, value); }
            }

            object IDataContainer.Get(TKey key)
            {
                if (this.data.TryGetValue(key, out var value))
                    return value;
                return null;
            }

            bool IDataContainer.TryGet(TKey key, out object value)
            {
                var result = this.data.TryGetValue(key, out var v);
                value = v;
                return result;
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

            public void Set(TKey key, T value)
            {
                this.data[key] = value;
            }

            public void Remove(TKey key)
            {
                data.Remove(key);
            }

            public void Clear()
            {
                data.Clear();
            }
        }

        private DataContainer<object> objectContainer = new DataContainer<object>();
        private Dictionary<Type, IDataContainer> structContainers = new Dictionary<Type, IDataContainer>();
        private Dictionary<TKey, IDataContainer> containerMap = new Dictionary<TKey, IDataContainer>();
        private Dictionary<TKey, List<Action<object, NotifyType>>> observerMap = new Dictionary<TKey, List<Action<object, NotifyType>>>();
        private List<KeyValuePair<TKey, Action<object, NotifyType>>> addObservers = new List<KeyValuePair<TKey, Action<object, NotifyType>>>();
        private List<KeyValuePair<TKey, Action<object, NotifyType>>> removeObservers = new List<KeyValuePair<TKey, Action<object, NotifyType>>>();
        private bool isNotifying;

        private Dictionary<string, Blackboard<TKey>> subBlackboards = new Dictionary<string, Blackboard<TKey>>();

        public Dictionary<string, Blackboard<TKey>> SubBlackboards
        {
            get { return subBlackboards; }
        }

        public T Get<T>(TKey key)
        {
            if (!this.containerMap.TryGetValue(key, out var dataContainer))
            {
                return default;
            }
            var type = typeof(T);
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

            var type = typeof(T);
            var isValueType = type.IsValueType;
            if (isValueType)
            {
                return ((DataContainer<T>)dataContainer).TryGet(key, out value);
            }

            var result = ((DataContainer<object>)dataContainer).TryGet(key, out var v);
            value = (T)v;
            return result;
        }

        public void Set<T>(TKey key, T value)
        {
            var type = typeof(T);
            var isValueType = type.IsValueType;
            var notifyType = NotifyType.Changed;
            if (!containerMap.TryGetValue(key, out var dataContainer))
            {
                notifyType = NotifyType.Added;
                if (isValueType)
                {
                    if (!structContainers.TryGetValue(type, out dataContainer))
                    {
                        structContainers[type] = dataContainer = new DataContainer<T>();
                    }

                    containerMap[key] = dataContainer;
                }
                else
                    containerMap[key] = dataContainer = objectContainer;
            }

            if (isValueType)
                ((DataContainer<T>)dataContainer).Set(key, value);
            else
                ((DataContainer<object>)dataContainer).Set(key, value);

            NotifyObservers(key, value, notifyType);
        }

        public void Remove(TKey key)
        {
            if (!containerMap.TryGetValue(key, out var dataContainer))
            {
                return;
            }

            NotifyObservers(key, dataContainer.Get(key), NotifyType.Remove);

            containerMap.Remove(key);
            dataContainer.Remove(key);
        }

        public void Clear()
        {
            objectContainer.Clear();
            structContainers.Clear();
            containerMap.Clear();
            observerMap.Clear();
            addObservers.Clear();
            removeObservers.Clear();
        }

        private void NotifyObservers(TKey key, object value, NotifyType notifyType)
        {
            if (!observerMap.TryGetValue(key, out var observers))
                return;

            addObservers.Clear();
            removeObservers.Clear();

            isNotifying = true;

            foreach (var observer in observers)
            {
                observer?.Invoke(value, notifyType);
            }

            isNotifying = false;

            foreach (var pair in removeObservers)
            {
                UnregisterObserver(pair.Key, pair.Value);
            }

            foreach (var pair in addObservers)
            {
                RegisterObserver(pair.Key, pair.Value);
            }

            addObservers.Clear();
            removeObservers.Clear();
        }

        public void RegisterObserver(TKey key, Action<object, NotifyType> observer)
        {
            if (isNotifying)
            {
                addObservers.Add(new KeyValuePair<TKey, Action<object, NotifyType>>(key, observer));
                return;
            }

            if (!observerMap.TryGetValue(key, out var observers))
            {
                return;
            }

            if (observers.Contains(observer))
            {
                return;
            }

            observers.Add(observer);
        }

        public void UnregisterObserver(TKey key, Action<object, NotifyType> observer)
        {
            if (isNotifying)
            {
                removeObservers.Add(new KeyValuePair<TKey, Action<object, NotifyType>>(key, observer));
                return;
            }

            if (!observerMap.TryGetValue(key, out var observers))
            {
                return;
            }

            observers.Remove(observer);
        }
    }
}