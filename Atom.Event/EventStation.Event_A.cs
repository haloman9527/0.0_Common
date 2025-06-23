using System;
using System.Collections.Generic;

namespace Atom
{
    public partial class EventStation<TKey>
    {
        public class Event<TArg> : EventBase, IEvent<TArg>
        {
            private readonly List<Action<TArg>> m_Handlers;

            public Event()
            {
                m_Handlers = new List<Action<TArg>>(8);
            }

            public void Add(Action<TArg> handler)
            {
                m_Handlers.Add(handler);
            }

            public void Remove(Action<TArg> handler)
            {
                m_Handlers.Remove(handler);
            }

            public void Clear()
            {
                m_Handlers.Clear();
            }

            public void Invoke(TArg arg)
            {
                for (int i = 0; i < m_Handlers.Count; i++)
                {
                    try
                    {
                        m_Handlers[i]?.Invoke(arg);
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