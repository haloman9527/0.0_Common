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
using System.Collections.Generic;
using Moyo;

namespace Moyo.Blackboard
{
    public enum NotifyType
    {
        Added,
        Changed,
        Remove
    }

    public class BBEventArg : BaseEventArg
    {
        public object value;
        public NotifyType notifyType;

        public override void OnSpawn()
        {
            value = null;
        }

        public override void OnRecycle()
        {
            value = null;
        }
    }

    public class BlackboardProcessor<TKey> : IBlackboard<TKey>
    {
        public Blackboard<TKey> blackboard;
        public Events<TKey> events;
        private List<KeyValuePair<TKey, Action<BBEventArg>>> addObservers;
        private List<KeyValuePair<TKey, Action<BBEventArg>>> removeObservers;
        private bool isNotifying;

        public BlackboardProcessor(Blackboard<TKey> blackboard) : this(blackboard, new Events<TKey>())
        {
        }

        public BlackboardProcessor(Blackboard<TKey> blackboard, Events<TKey> events)
        {
            this.blackboard = blackboard;
            this.events = events;
            this.events = new Events<TKey>();
            this.addObservers = new List<KeyValuePair<TKey, Action<BBEventArg>>>();
            this.removeObservers = new List<KeyValuePair<TKey, Action<BBEventArg>>>();
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

        public bool Set<T>(TKey key, T value)
        {
            var notifyType = NotifyType.Changed;
            if (!blackboard.Contains(key))
            {
                notifyType = NotifyType.Added;
            }

            if (blackboard.Set(key, value))
            {
                NotifyObservers(key, value, notifyType);
                return true;
            }

            return false;
        }

        public bool Remove(TKey key)
        {
            if (!blackboard.TryGet(key, out var value))
            {
                return false;
            }

            blackboard.Remove(key);
            NotifyObservers(key, value, NotifyType.Remove);
            return true;
        }

        public void Clear()
        {
            blackboard.Clear();
            events.Clear();
            addObservers.Clear();
            removeObservers.Clear();
        }

        private void NotifyObservers(TKey key, object value, NotifyType notifyType)
        {
            if (!events.Check(key))
                return;

            addObservers.Clear();
            removeObservers.Clear();

            isNotifying = true;
            try
            {
                using (var bbEvtArg = ObjectPools.Spawn<BBEventArg>())
                {
                    bbEvtArg.value = value;
                    bbEvtArg.notifyType = notifyType;
                    events.Publish(key, bbEvtArg);
                }
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

        public void RegisterObserver(TKey key, Action<BBEventArg> observer)
        {
            if (isNotifying)
            {
                addObservers.Add(new KeyValuePair<TKey, Action<BBEventArg>>(key, observer));
                return;
            }

            events.Subscribe(key, observer);
        }

        public void UnregisterObserver(TKey key, Action<BBEventArg> observer)
        {
            if (isNotifying)
            {
                removeObservers.Add(new KeyValuePair<TKey, Action<BBEventArg>>(key, observer));
                return;
            }

            events.Unsubscribe(key, observer);
        }
    }
}