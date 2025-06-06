using System;
using System.Collections.Generic;

namespace Atom
{
    public partial class EventStation<TKey>
    {
        private readonly Dictionary<TKey, EventBase> m_Events;

        public EventStation()
        {
            m_Events = new Dictionary<TKey, EventBase>();
        }
        
        public bool HasEvent(TKey key)
        {
            return m_Events.ContainsKey(key);
        }

        public object GetEvent(TKey key)
        {
            m_Events.TryGetValue(key, out var evt);
            return evt;
        }
        
        public void RegisterEvent(TKey key, EventBase evt)
        {
            if (HasEvent(key))
                throw new InvalidOperationException($"Event '{key}' already registered");
            
            m_Events.Add(key, evt);
        }

        public void UnRegisterEvent(TKey key)
        {
            m_Events.Remove(key);
        }

        public void UnRegisterAllEvents()
        {
            m_Events.Clear();
        }

        public void Subscribe(TKey key, Action handler)
        {
            var evt = GetEvent(key);
            if (evt is not Event tmpEvt)
            {
                if (evt != null)
                    throw new InvalidOperationException($"Event '{key}' type mismatch, type '{evt.GetType()}'");
                
                tmpEvt = new Event();
                RegisterEvent(key, tmpEvt);
            }

            tmpEvt.Add(handler);
        }

        public void Subscribe<TArg>(TKey key, Action<TArg> handler)
        {
            var evt = GetEvent(key);
            if (evt is not Event<TArg> tmpEvt)
            {
                if (evt != null)
                    throw new InvalidOperationException($"Event '{key}' type mismatch, type '{evt.GetType()}'");
                
                tmpEvt = new Event<TArg>();
                RegisterEvent(key, tmpEvt);
            }

            tmpEvt.Add(handler);
        }

        public void Unsubscribe(TKey key, Action handler)
        {
            var evt = GetEvent(key);
            if (evt == null)
                throw new InvalidOperationException($"can not found event '{key}'");

            if (evt is not Event tmpEvt)
                throw new InvalidOperationException("Event type mismatch.");

            tmpEvt.Remove(handler);
        }
        
        public void Unsubscribe<TArg>(TKey key, Action<TArg> handler)
        {
            var evt = GetEvent(key);
            if (evt == null)
                throw new InvalidOperationException($"can not found event '{key}'");

            if (evt is not Event<TArg> tmpEvt)
                throw new InvalidOperationException("Event type mismatch.");
            
            tmpEvt.Remove(handler);
        }

        public void Publish<TArg>(TKey key, TArg arg)
        {
            var evt = GetEvent(key);
            if (evt == null)
                throw new InvalidOperationException($"can not found event '{key}'");

            if (evt is not IEvent<TArg> tmpEvt)
                throw new InvalidOperationException("Event type mismatch.");
            
            tmpEvt.Invoke(arg);
        }

        public void Publish(TKey key)
        {
            var evt = GetEvent(key);
            if (evt == null)
                throw new InvalidOperationException($"can not found event '{key}'");

            if (evt is not Event tmpEvt)
                throw new InvalidOperationException("Event type mismatch.");

            tmpEvt.Invoke();
        }
    }
}