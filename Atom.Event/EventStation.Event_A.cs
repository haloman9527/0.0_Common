using System;
using System.Collections.Generic;

namespace Atom
{
    public partial class EventStation<TKey>
    {
        public class Event<TArg> : EventBase, IEvent<TArg>
        {
            private readonly List<Action<TArg>> m_Handlers = new(8);
            private readonly Queue<Action<TArg>> m_HandlerQueue = new(8);

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

            public void Invoke(in TArg arg)
            {
                m_HandlerQueue.Clear();
                for (int i = 0; i < m_Handlers.Count; i++)
                {
                    m_HandlerQueue.Enqueue(m_Handlers[i]);
                }

                while (m_HandlerQueue.Count > 0)
                {
                    try
                    {
                        m_HandlerQueue.Dequeue()?.Invoke(arg);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }

                m_HandlerQueue.Clear();
            }
        }
    }
}