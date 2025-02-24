using System;
using System.Collections.Generic;
using Moyo.Internal;

namespace Moyo
{
    public class EventService<TKey>
    {
        private readonly Dictionary<TKey, IEvent> m_events = new Dictionary<TKey, IEvent>();
        
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

        public void Subscribe<TArg>(TKey key, Action<TArg> handler)
        {
            if (!m_events.TryGetValue(key, out var evt))
            {
                m_events[key] = evt = new Moyo.Internal.Event<TArg>();
            }

            if (evt is Moyo.Internal.Event<TArg> _evt)
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

            if (evt is Moyo.Internal.Event<TArg> _evt)
            {
                _evt.Remove(handler);
            }
            else
            {
                throw new InvalidOperationException("Event type mismatch.");
            }
        }

        public void Publish<TArg>(TKey key, TArg arg)
        {
            if (!m_events.TryGetValue(key, out var evt))
            {
                return;
            }

            if (evt is Moyo.Internal.Event<TArg> _evt)
            {
                _evt.Invoke(arg);
            }
        }
        
        public void Subscribe(TKey key, Action handler)
        {
            if (!m_events.TryGetValue(key, out var evt))
            {
                m_events[key] = evt = new Moyo.Internal.Event();
            }

            if (evt is Moyo.Internal.Event _evt)
            {
                _evt.Add(handler);
            }
            else
            {
                throw new InvalidOperationException("Event type mismatch.");
            }
        }

        public void Unsubscribe(TKey key, Action handler)
        {
            if (!m_events.TryGetValue(key, out var evt))
            {
                return;
            }

            if (evt is Moyo.Internal.Event _evt)
            {
                _evt.Remove(handler);
            }
            else
            {
                throw new InvalidOperationException("Event type mismatch.");
            }
        }

        public void Publish(TKey key)
        {
            if (!m_events.TryGetValue(key, out var evt))
            {
                return;
            }

            if (evt is Moyo.Internal.Event _evt)
            {
                _evt.Invoke();
            }
        }
    }

    public class EventService
    {
        private readonly Dictionary<Type, IEvent> m_events = new Dictionary<Type, IEvent>();

        public bool Check(Type e)
        {
            return m_events.TryGetValue(e, out var evts) && !evts.IsNull;
        }

        public bool Check<E>()
        {
            return m_events.TryGetValue(TypeCache<E>.TYPE, out var evts) && !evts.IsNull;
        }

        public void Remove(Type e)
        {
            m_events.Remove(e);
        }

        public void Remove<E>()
        {
            m_events.Remove(TypeCache<E>.TYPE);
        }

        public void Clear()
        {
            m_events.Clear();
        }
        
        public void Subscribe<E>(Action<E> handler)
        {
            if (!m_events.TryGetValue(TypeCache<E>.TYPE, out var evt))
            {
                m_events[TypeCache<E>.TYPE] = evt = new Event<E>();
            }

            if (evt is Moyo.Internal.Event<E> _evt)
            {
                _evt.Add(handler);
            }
            else
            {
                throw new InvalidOperationException("Event type mismatch.");
            }
        }

        public void Unsubscribe<E>(Action<E> handler)
        {
            if (!m_events.TryGetValue(TypeCache<E>.TYPE, out var evt))
            {
                return;
            }

            if (evt is Moyo.Internal.Event<E> _evt)
            {
                _evt.Remove(handler);
            }
            else
            {
                throw new InvalidOperationException("Event type mismatch.");
            }
        }

        public void Publish<E>(E e)
        {
            if (!m_events.TryGetValue(TypeCache<E>.TYPE, out var evt))
            {
                return;
            }

            if (evt is Moyo.Internal.Event<E> _evt)
            {
                _evt.Invoke(e);
            }
        }
    }
}