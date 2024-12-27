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
        private class Event : IEvent
        {
            private readonly List<WeakReference<Action>> handlers = new List<WeakReference<Action>>(8);

            public bool IsNull => handlers.Count == 0;

            public void Add(Action handler)
            {
                handlers.Add(new WeakReference<Action>(handler));
            }

            public void Remove(Action handler)
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

            public void Invoke()
            {
                for (int i = handlers.Count - 1; i >= 0; i--)
                {
                    if (handlers[i].TryGetTarget(out var handler))
                    {
                        try
                        {
                            handler?.Invoke();
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

        private class Event<T0, T1> : IEvent
        {
            private readonly List<WeakReference<Action<T0, T1>>> handlers = new List<WeakReference<Action<T0, T1>>>(8);

            public bool IsNull => handlers.Count == 0;

            public void Add(Action<T0, T1> handler)
            {
                handlers.Add(new WeakReference<Action<T0, T1>>(handler));
            }

            public void Remove(Action<T0, T1> handler)
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

            public void Invoke(T0 arg0, T1 arg1)
            {
                for (int i = handlers.Count - 1; i >= 0; i--)
                {
                    if (handlers[i].TryGetTarget(out var handler))
                    {
                        try
                        {
                            handler?.Invoke(arg0, arg1);
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
        public void Subscribe(TKey key, Action handler)
        {
            if (!events.TryGetValue(key, out var evt))
            {
                events[key] = evt = new Event();
            }

            if (evt is Event _evt)
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
            if (!events.TryGetValue(key, out var evt))
            {
                return;
            }

            if (evt is Event _evt)
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
            if (!events.TryGetValue(key, out var evt))
            {
                return;
            }

            if (evt is Event _evt)
            {
                _evt.Invoke();
            }
        }
    }

    public partial class Events<TKey>
    {
        public void Subscribe<T0>(TKey key, Action<T0> handler)
        {
            if (!events.TryGetValue(key, out var evt))
            {
                events[key] = evt = new Event<T0>();
            }

            if (evt is Event<T0> _evt)
            {
                _evt.Add(handler);
            }
            else
            {
                throw new InvalidOperationException("Event type mismatch.");
            }
        }

        public void Unsubscribe<T0>(TKey key, Action<T0> handler)
        {
            if (!events.TryGetValue(key, out var evt))
            {
                return;
            }

            if (evt is Event<T0> _evt)
            {
                _evt.Remove(handler);
            }
            else
            {
                throw new InvalidOperationException("Event type mismatch.");
            }
        }

        public void Publish<T0>(TKey key, T0 arg)
        {
            if (!events.TryGetValue(key, out var evt))
            {
                return;
            }

            if (evt is Event<T0> _evt)
            {
                _evt.Invoke(arg);
            }
        }
    }

    public partial class Events<TKey>
    {
        public void Subscribe<T0, T1>(TKey key, Action<T0, T1> handler)
        {
            if (!events.TryGetValue(key, out var evt))
            {
                events[key] = evt = new Event<T0, T1>();
            }

            if (evt is Event<T0, T1> _evt)
            {
                _evt.Add(handler);
            }
            else
            {
                throw new InvalidOperationException("Event type mismatch.");
            }
        }

        public void Unsubscribe<T0, T1>(TKey key, Action<T0, T1> handler)
        {
            if (!events.TryGetValue(key, out var evt))
            {
                return;
            }

            if (evt is Event<T0, T1> _evt)
            {
                _evt.Remove(handler);
            }
            else
            {
                throw new InvalidOperationException("Event type mismatch.");
            }
        }

        public void Publish<T0, T1>(TKey key, T0 arg0, T1 arg1)
        {
            if (!events.TryGetValue(key, out var evt))
            {
                return;
            }

            if (evt is Event<T0, T1> _evt)
            {
                _evt.Invoke(arg0, arg1);
            }
        }
    }
}