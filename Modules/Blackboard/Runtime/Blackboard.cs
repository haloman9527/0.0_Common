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
                    return value;
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

        private DataContainer<object> objectDataContainer = new DataContainer<object>();
        private Dictionary<TKey, IDataContainer> keyContainerMap = new Dictionary<TKey, IDataContainer>();
        private Dictionary<Type, IDataContainer> structDataContainers = new Dictionary<Type, IDataContainer>();
        private Dictionary<TKey, List<Action<object, NotifyType>>> dataObserversMap = new Dictionary<TKey, List<Action<object, NotifyType>>>();

        public T Get<T>(TKey key)
        {
            if (!this.keyContainerMap.TryGetValue(key, out var dataContainer))
                return default;
            var type = typeof(T);
            var isValueType = type.IsValueType;
            if (isValueType)
                return ((DataContainer<T>)dataContainer).Get(key);
            else
                return (T)((DataContainer<object>)dataContainer).Get(key);
        }

        public bool TryGet<T>(TKey key, out T value)
        {
            if (!this.keyContainerMap.TryGetValue(key, out var dataContainer))
            {
                value = default;
                return false;
            }

            var type = typeof(T);
            var isValueType = type.IsValueType;
            if (isValueType)
                return ((DataContainer<T>)dataContainer).TryGet(key, out value);
            else
            {
                var result = ((DataContainer<object>)dataContainer).TryGet(key, out var v);
                value = (T)v;
                return result;
            }
        }

        public void Set<T>(TKey key, T value)
        {
            var type = typeof(T);
            var isValueType = type.IsValueType;
            var exists = true;
            if (!keyContainerMap.TryGetValue(key, out var dataContainer))
            {
                exists = false;
                if (isValueType)
                {
                    if (!structDataContainers.TryGetValue(type, out dataContainer))
                        structDataContainers[type] = dataContainer = new DataContainer<T>();

                    keyContainerMap[key] = dataContainer;
                }
                else
                    keyContainerMap[key] = dataContainer = objectDataContainer;
            }

            if (isValueType)
                ((DataContainer<T>)dataContainer).Set(key, value);
            else
                ((DataContainer<object>)dataContainer).Set(key, value);
            if (dataObserversMap.TryGetValue(key, out var observers))
            {
                var notifyType = exists ? NotifyType.Changed : NotifyType.Added;
                foreach (var observer in observers)
                {
                    observer?.Invoke(value, notifyType);
                }
            }
        }

        public void Remove(TKey key)
        {
            if (!keyContainerMap.TryGetValue(key, out var dataContainer))
                return;
            if (dataObserversMap.TryGetValue(key, out var observers))
            {
                foreach (var observer in observers)
                {
                    observer?.Invoke(dataContainer.Get(key), NotifyType.Remove);
                }
            }

            keyContainerMap.Remove(key);
            dataContainer.Remove(key);
        }

        public void Clear()
        {
            objectDataContainer.Clear();
            structDataContainers.Clear();
            keyContainerMap.Clear();
            dataObserversMap.Clear();
        }

        public void RegisterObserver(TKey key, Action<object, NotifyType> observer)
        {
            if (!dataObserversMap.TryGetValue(key, out var observers))
                dataObserversMap[key] = observers = new List<Action<object, NotifyType>>();
            if (observers.Contains(observer))
                return;
            observers.Add(observer);
        }

        public void UnregisterObserver(TKey key, Action<object, NotifyType> observer)
        {
            if (!dataObserversMap.TryGetValue(key, out var observers))
                return;
            observers.Remove(observer);
        }
    }
}