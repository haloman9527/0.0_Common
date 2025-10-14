using System.Collections.Generic;

namespace Atom
{
    public sealed class EventQueueStation
    {
        private Queue<QueueEventBase> m_EventQueue = new  Queue<QueueEventBase>();

        public void Update()
        {
            if (m_EventQueue.Count > 0)
            {
                var cur = m_EventQueue.Peek();
                if (cur.Completed)
                {
                    m_EventQueue.Dequeue();
                    if (m_EventQueue.Count > 0)
                    {
                        var next = m_EventQueue.Peek();
                        next.Invoke();
                    }
                }
            }
        }
        
        public void PushEvent(QueueEventBase evt)
        {
            m_EventQueue.Enqueue(evt);
            if (m_EventQueue.Count == 1)
                evt.Invoke();
        }

        public void Clear()
        {
            if (m_EventQueue.Count > 0)
            {
                var cur = m_EventQueue.Peek();
                cur.Abort();
            }
            
            m_EventQueue.Clear();
        }
    }
}