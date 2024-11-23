using System;
using System.Collections.Generic;

namespace Jiange
{
    public interface IEvent
    {
        bool IsNull { get; }
    }

    public partial class Events<TKey>
    {
        private class Event : IEvent
        {
            public event Action handler;

            public bool IsNull => handler == null;

            public void Handle()
            {
                handler?.Invoke();
            }
        }

        private class Event<T> : IEvent
        {
            public event Action<T> handler;

            public bool IsNull => handler == null;

            public void Handle(T arg)
            {
                handler?.Invoke(arg);
            }
        }

        private class Event<T0, T1> : IEvent
        {
            public event Action<T0, T1> handler;

            public bool IsNull => handler == null;

            public void Handle(T0 arg0, T1 arg1)
            {
                handler?.Invoke(arg0, arg1);
            }
        }

        private readonly Dictionary<TKey, IEvent> events = new Dictionary<TKey, IEvent>();

        public void Subscribe(TKey key, Action handler)
        {
            if (!events.TryGetValue(key, out var evts))
            {
                events[key] = evts = new Event();
            }

            ((Event)evts).handler += handler;
        }

        public void Unsubscribe(TKey key, Action handler)
        {
            if (!events.TryGetValue(key, out var evts))
            {
                return;
            }

            ((Event)evts).handler -= handler;
        }

        public void Publish(TKey key)
        {
            if (!events.TryGetValue(key, out var evts))
            {
                return;
            }

            ((Event)evts).Handle();
        }

        public void Subscribe<T0>(TKey key, Action<T0> handler)
        {
            if (!events.TryGetValue(key, out var evts))
            {
                events[key] = evts = new Event<T0>();
            }

            ((Event<T0>)evts).handler += handler;
        }

        public void Unsubscribe<T0>(TKey key, Action<T0> handler)
        {
            if (!events.TryGetValue(key, out var evts))
            {
                return;
            }

            ((Event<T0>)evts).handler -= handler;
        }

        public void Publish<T0>(TKey key, T0 arg)
        {
            if (!events.TryGetValue(key, out var evts))
            {
                return;
            }

            ((Event<T0>)evts).Handle(arg);
        }


        public bool ExistsEvent(TKey key)
        {
            return events.TryGetValue(key, out var evts) && !evts.IsNull;
        }

        public void Remove(TKey key)
        {
            events.Remove(key);
        }

        public void Clear()
        {
            events.Clear();
        }
    }

    public partial class Events<TKey>
    {
        public void Subscribe<T0, T1>(TKey key, Action<T0, T1> handler)
        {
            if (!events.TryGetValue(key, out var evts))
            {
                events[key] = evts = new Event<T0, T1>();
            }

            ((Event<T0, T1>)evts).handler += handler;
        }

        public void Unsubscribe<T0, T1>(TKey key, Action<T0, T1> handler)
        {
            if (!events.TryGetValue(key, out var evts))
            {
                return;
            }

            ((Event<T0, T1>)evts).handler -= handler;
        }

        public void Publish<T0, T1>(TKey key, T0 arg0, T1 arg1)
        {
            if (!events.TryGetValue(key, out var evts))
            {
                return;
            }

            ((Event<T0, T1>)evts).Handle(arg0, arg1);
        }
    }
}