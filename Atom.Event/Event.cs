using System;
using System.Collections.Generic;

namespace Atom.Internal
{
    public interface IEvent
    {
        bool IsNull { get; }

        void Clear();
    }
    
    public interface IGlobalEvent
    {
        public Type EventType { get; }
    }

    public class Event<TArg> : IEvent
    {
        private readonly List<WeakReference<Action<TArg>>> handlers = new List<WeakReference<Action<TArg>>>(8);

        public bool IsNull => handlers.Count == 0;

        public void Add(Action<TArg> handler)
        {
            handlers.Add(new WeakReference<Action<TArg>>(handler));
        }

        public void Remove(Action<TArg> handler)
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

        public void Invoke(TArg arg)
        {
            for (int i = 0; i < handlers.Count; i++)
            {
                if (handlers[i].TryGetTarget(out var handler))
                {
                    try
                    {
                        handler.Invoke(arg);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
                else
                {
                    handlers.RemoveAt(i--);
                }
            }
        }

        public void Clear()
        {
            handlers.Clear();
        }
    }

    public class Event : IEvent
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
            for (int i = 0; i < handlers.Count; i++)
            {
                if (handlers[i].TryGetTarget(out var handler))
                {
                    try
                    {
                        handler.Invoke();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
                else
                {
                    handlers.RemoveAt(i--);
                }
            }
        }

        public void Clear()
        {
            handlers.Clear();
        }
    }
}