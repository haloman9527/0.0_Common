using System;
using System.Collections.Generic;

namespace Moyo
{
    public interface IEvent
    {
        bool IsNull { get; }

        void Clear();
    }

    public class Event<T> : IEvent
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

        public void Invoke(T arg)
        {
            for (int i = handlers.Count - 1; i >= 0; i--)
            {
                if (handlers[i].TryGetTarget(out var handler))
                {
                    try
                    {
                        handler?.Invoke(arg);
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