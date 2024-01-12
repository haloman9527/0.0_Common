using System;
using System.Collections.Generic;

namespace CZToolKit
{
    public class Events<TKey>
    {
        public interface IEvent
        {
            bool IsNull { get; }
        }

        private class Event : IEvent
        {
            public event Action handler;

            public bool IsNull => handler == null;

            public void Handle()
            {
                handler?.Invoke();
            }
        }

        private class Event<T> : IEvent where T : struct
        {
            public event Action<T> handler;

            public bool IsNull => handler == null;

            public void Handle(T arg)
            {
                handler?.Invoke(arg);
            }
        }

        private Dictionary<TKey, IEvent> events = new Dictionary<TKey, IEvent>();

        public void RegisterEvent<T>(TKey key, Action<T> handler) where T : struct
        {
            if (!events.TryGetValue(key, out var evts))
            {
                events[key] = evts = new Event<T>();
            }

            ((Event<T>)evts).handler += handler;
        }

        public void UnregisterEvent<T>(TKey key, Action<T> handler) where T : struct
        {
            if (!events.TryGetValue(key, out var evts))
            {
                return;
            }

            ((Event<T>)evts).handler -= handler;
        }

        public void Invoke<T>(TKey key, T arg) where T : struct
        {
            if (!events.TryGetValue(key, out var evts))
            {
                return;
            }

            ((Event<T>)evts).Handle(arg);
        }

        public void RegisterEvent(TKey key, Action handler)
        {
            if (!events.TryGetValue(key, out var evts))
            {
                events[key] = evts = new Event();
            }

            ((Event)evts).handler += handler;
        }

        public void UnregisterEvent(TKey key, Action handler)
        {
            if (!events.TryGetValue(key, out var evts))
            {
                return;
            }

            ((Event)evts).handler -= handler;
        }

        public void Invoke(TKey key)
        {
            if (!events.TryGetValue(key, out var evts))
            {
                return;
            }

            ((Event)evts).Handle();
        }

        public bool HasEvent(TKey key)
        {
            return events.ContainsKey(key);
        }

        public void Clear()
        {
            events.Clear();
        }
    }
}