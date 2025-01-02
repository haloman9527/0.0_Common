using System;
using System.Collections.Generic;

namespace Moyo
{
    public interface IEvent
    {
        bool IsNull { get; }

        void Clear();
    }

    public partial class Events<TKey>
    {
        private class Event<T> : IEvent
        {
            private readonly List<WeakReference<Action<T>>> handlers = new List<WeakReference<Action<T>>>(8);

            public bool IsNull => handlers.Count == 0;

            public void Add(Action<T> handler)
            {
                handlers.Add(new WeakReference<Action<T>>(handler));
            }

            public void Remove(Action<T> handler)
            {
                for (int i = handlers.Count - 1; i >= 0; i--)
                {
                    if (handlers[i].TryGetTarget(out var existingHandler) && existingHandler == handler)
                    {
                        handlers.RemoveAt(i);
                        break;
                    }
                }
            }

            public void Invoke(T arg0)
            {
                for (int i = handlers.Count - 1; i >= 0; i--)
                {
                    if (handlers[i].TryGetTarget(out var handler))
                    {
                        try
                        {
                            handler?.Invoke(arg0);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                        }
                    }
                    else
                    {
                        handlers.RemoveAt(i);
                    }
                }
            }

            public void Clear()
            {
                handlers.Clear();
            }
        }
    }

    public partial class Events<TKey>
    {
        private readonly Dictionary<TKey, IEvent> events = new Dictionary<TKey, IEvent>();

        public void Subscribe<TArg>(TKey key, Action<TArg> handler) where TArg : BaseEventArg
        {
            if (!events.TryGetValue(key, out var evt))
            {
                events[key] = evt = new Event<TArg>();
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
            if (!events.TryGetValue(key, out var evt))
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
            if (!events.TryGetValue(key, out var evt))
            {
                return;
            }

            if (evt is Event<TArg> _evt)
            {
                _evt.Invoke(e);
            }

            ObjectPools.Recycle(e);
        }

        public bool Check(TKey key)
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
}