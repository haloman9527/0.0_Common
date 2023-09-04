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
 *  Blog: https://www.mindgear.net/
 *
 */

#endregion

using System;
using System.Collections.Generic;

namespace CZToolKit.Blackboard
{
    public enum NotifyType
    {
        Added,
        Changed,
        Remove
    }

    public class BlackboardVM<TKey> : IBlackboard<TKey>
    {
        public Blackboard<TKey> blackboard;
        private Dictionary<TKey, List<Action<object, NotifyType>>> observerMap = new Dictionary<TKey, List<Action<object, NotifyType>>>();
        private List<KeyValuePair<TKey, Action<object, NotifyType>>> addObservers = new List<KeyValuePair<TKey, Action<object, NotifyType>>>();
        private List<KeyValuePair<TKey, Action<object, NotifyType>>> removeObservers = new List<KeyValuePair<TKey, Action<object, NotifyType>>>();
        private bool isNotifying;

        public BlackboardVM(Blackboard<TKey> blackboard)
        {
            this.blackboard = blackboard;
        }

        public bool Contains(TKey key)
        {
            return blackboard.Contains(key);
        }

        public T Get<T>(TKey key)
        {
            return blackboard.Get<T>(key);
        }

        public object Get(TKey key)
        {
            return blackboard.Get(key);
        }

        public bool TryGet<T>(TKey key, out T value)
        {
            return blackboard.TryGet(key, out value);
        }

        public bool TryGet(TKey key, out object value)
        {
            return blackboard.TryGet(key, out value);
        }

        public void Set<T>(TKey key, T value)
        {
            var notifyType = NotifyType.Changed;
            if (!blackboard.Contains(key))
                notifyType = NotifyType.Added;
            blackboard.Set(key, value);
            NotifyObservers(key, value, notifyType);
        }

        public void Remove(TKey key)
        {
            if (blackboard.TryGet(key, out var value))
            {
                NotifyObservers(key, value, NotifyType.Remove);
            }

            blackboard.Remove(key);
        }

        public void Clear()
        {
            blackboard.Clear();
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
            try
            {
                foreach (var observer in observers)
                {
                    observer?.Invoke(value, notifyType);
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                isNotifying = false;
            }

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