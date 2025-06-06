using System;
using System.Collections.Generic;

namespace Atom
{
    public partial class EventStation<TKey>
    {
        public class Event<TArg> : EventBase, IEvent<TArg>
        {
            private readonly List<WeakReference<Action<TArg>>> m_Handlers;

            public Event()
            {
                m_Handlers = new List<WeakReference<Action<TArg>>>(8);
            }

            public Event(List<WeakReference<Action<TArg>>> mHandlers)
            {
                m_Handlers = mHandlers;
            }

            public void Add(Action<TArg> handler)
            {
                m_Handlers.Add(new WeakReference<Action<TArg>>(handler));
            }

            public void Remove(Action<TArg> handler)
            {
                for (int i = m_Handlers.Count - 1; i >= 0; i--)
                {
                    if (m_Handlers[i].TryGetTarget(out var existingHandler) && existingHandler == handler)
                    {
                        m_Handlers.RemoveAt(i);
                        break;
                    }
                }
            }

            public void Clear()
            {
                m_Handlers.Clear();
            }

            public void Invoke(TArg arg)
            {
                for (int i = 0; i < m_Handlers.Count; i++)
                {
                    if (m_Handlers[i].TryGetTarget(out var handler))
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
                        m_Handlers.RemoveAt(i--);
                    }
                }
            }
        }
    }
}