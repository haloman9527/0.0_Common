using System;
using System.Collections.Generic;

namespace Moyo
{
    public partial class EventService<TKey>
    {
        public bool Check(TKey key)
        {
            return m_events.TryGetValue(key, out var evts) && !evts.IsNull;
        }

        public void Remove(TKey key)
        {
            m_events.Remove(key);
        }

        public void Clear()
        {
            m_events.Clear();
        }
    }

    public partial class EventService<TKey>
    {
        private readonly Dictionary<TKey, IEvent> m_events = new Dictionary<TKey, IEvent>();

        public void Subscribe<TArg>(TKey key, Action<TArg> handler)
        {
            if (!m_events.TryGetValue(key, out var evt))
            {
                m_events[key] = evt = new Event<TArg>();
            }

            if (evt is Event<TArg> _evt)
            {
                _evt.Add(handler);
            }
            else
            {
                throw new InvalidOperationException("Event type mismatch.");
            }
        }

        public void Unsubscribe<TArg>(TKey key, Action<TArg> handler)
        {
            if (!m_events.TryGetValue(key, out var evt))
            {
                return;
            }

            if (evt is Event<TArg> _evt)
            {
                _evt.Remove(handler);
            }
            else
            {
                throw new InvalidOperationException("Event type mismatch.");
            }
        }

        public void Publish<TArg>(TKey key, TArg e)
        {
            if (!m_events.TryGetValue(key, out var evt))
            {
                return;
            }

            if (evt is Event<TArg> _evt)
            {
                _evt.Invoke(e);
            }

            ObjectPoolService.Recycle(e);
        }
    }
}