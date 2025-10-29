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

namespace Atom
{
    public enum NotifyType
    {
        Added,
        Changed,
        Remove
    }

    public struct BBEventArg
    {
        public object Value;
        public NotifyType NotifyType;
    }

    public class BlackboardProcessor<TKey> : IBlackboard<TKey>
    {
        private Blackboard<TKey> m_Blackboard;
        private EventStation<TKey> m_Events;
        private List<KeyValuePair<TKey, Action<BBEventArg>>> m_AddObservers;
        private List<KeyValuePair<TKey, Action<BBEventArg>>> m_RemoveObservers;
        private bool m_IsNotifying;

        public Blackboard<TKey> Blackboard
        {
            get { return m_Blackboard; }
        }

        public EventStation<TKey> Events
        {
            get { return m_Events; }
        }
        
        public BlackboardProcessor(Blackboard<TKey> blackboard) : this(blackboard, new EventStation<TKey>())
        {
        }

        public BlackboardProcessor(Blackboard<TKey> blackboard, EventStation<TKey> events)
        {
            this.m_Blackboard = blackboard;
            this.m_Events = events;
            this.m_AddObservers = new List<KeyValuePair<TKey, Action<BBEventArg>>>();
            this.m_RemoveObservers = new List<KeyValuePair<TKey, Action<BBEventArg>>>();
        }

        public bool Contains(TKey key)
        {
            return m_Blackboard.Contains(key);
        }

        public T Get<T>(TKey key)
        {
            return m_Blackboard.Get<T>(key);
        }

        public object Get(TKey key)
        {
            return m_Blackboard.Get(key);
        }

        public bool TryGet<T>(TKey key, out T value)
        {
            return m_Blackboard.TryGet(key, out value);
        }

        public bool TryGet(TKey key, out object value)
        {
            return m_Blackboard.TryGet(key, out value);
        }

        public bool Set<T>(TKey key, T value)
        {
            var notifyType = NotifyType.Changed;
            if (!m_Blackboard.Contains(key))
            {
                notifyType = NotifyType.Added;
            }

            if (m_Blackboard.Set(key, value))
            {
                NotifyObservers(key, value, notifyType);
                return true;
            }

            return false;
        }

        public bool Remove(TKey key)
        {
            if (!m_Blackboard.TryGet(key, out var value))
            {
                return false;
            }

            m_Blackboard.Remove(key);
            NotifyObservers(key, value, NotifyType.Remove);
            return true;
        }

        public void Clear()
        {
            m_Blackboard.Clear();
            m_Events.UnregisterAll();
            m_AddObservers.Clear();
            m_RemoveObservers.Clear();
        }

        private void NotifyObservers(TKey key, object value, NotifyType notifyType)
        {
            if (!m_Events.HasEvent(key))
                return;

            m_AddObservers.Clear();
            m_RemoveObservers.Clear();

            m_IsNotifying = true;
            try
            {
                m_Events.Publish(key, new BBEventArg() { Value = value, NotifyType = notifyType });
            }
            finally
            {
                m_IsNotifying = false;
            }

            foreach (var pair in m_RemoveObservers)
            {
                UnregisterObserver(pair.Key, pair.Value);
            }

            foreach (var pair in m_AddObservers)
            {
                RegisterObserver(pair.Key, pair.Value);
            }

            m_AddObservers.Clear();
            m_RemoveObservers.Clear();
        }

        public void RegisterObserver(TKey key, Action<BBEventArg> observer)
        {
            if (m_IsNotifying)
            {
                m_AddObservers.Add(new KeyValuePair<TKey, Action<BBEventArg>>(key, observer));
                return;
            }

            m_Events.Subscribe(key, observer);
        }

        public void UnregisterObserver(TKey key, Action<BBEventArg> observer)
        {
            if (m_IsNotifying)
            {
                m_RemoveObservers.Add(new KeyValuePair<TKey, Action<BBEventArg>>(key, observer));
                return;
            }

            m_Events.Unsubscribe(key, observer);
        }
    }
}