using System;
using System.Collections.Generic;

namespace Atom
{
    public partial class EventStation<TKey>
    {
        public class Event : EventBase, IEvent
        {
            private readonly List<WeakReference<Action>> m_Handlers;

            public Event()
            {
                m_Handlers = new List<WeakReference<Action>>(8);
            }

            public void Add(Action handler)
            {
                m_Handlers.Add(new WeakReference<Action>(handler));
            }

            public void Remove(Action handler)
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

            public void Invoke()
            {
                for (int i = 0; i < m_Handlers.Count; i++)
                {
                    if (m_Handlers[i].TryGetTarget(out var handler))
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
                        m_Handlers.RemoveAt(i--);
                    }
                }
            }
        }
    }
}