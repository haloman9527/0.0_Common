using System;
using System.Collections.Generic;

namespace Atom
{
    public partial class EventStation<TKey>
    {
        public class Event : EventBase, IEvent
        {
            private readonly List<Action> m_Handlers;

            public Event()
            {
                m_Handlers = new List<Action>(8);
            }

            public void Add(Action handler)
            {
                m_Handlers.Add(handler);
            }

            public void Remove(Action handler)
            {
                m_Handlers.Remove(handler);
            }

            public void Clear()
            {
                m_Handlers.Clear();
            }

            public void Invoke()
            {
                for (int i = 0; i < m_Handlers.Count; i++)
                {
                    try
                    {
                        m_Handlers[i]?.Invoke();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
        }
    }
}