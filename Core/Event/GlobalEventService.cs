using System;
using System.Collections.Generic;
using Atom.Internal;

namespace Atom
{
    public partial class GlobalEventService
    {
        public static GlobalEventService Instance => s_Instance;

        static GlobalEventService()
        {
            s_allGlobalEvents = new Dictionary<Type, List<IGlobalEvent>>(128);

            foreach (var type in TypesCache.GetTypesDerivedFrom<IGlobalEvent>())
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                if (!type.IsClass)
                {
                    continue;
                }

                var evt = Activator.CreateInstance(type) as IGlobalEvent;
                if (evt == null)
                {
                    continue;
                }

                var evtType = evt.EventType;
                if (!s_allGlobalEvents.TryGetValue(evtType, out var evts))
                {
                    s_allGlobalEvents[evtType] = evts = new List<IGlobalEvent>();
                }

                evts.Add(evt);
            }
        }

        private abstract class EventTask
        {
            public abstract void Invoke(EventService<Type> events);

            public abstract void Reset();
        }

        private class EventTask<E> : EventTask
        {
            public E e;

            public override void Invoke(EventService<Type> events)
            {
                events.Publish(TypeCache<E>.TYPE, e);
            }

            public override void Reset()
            {
                e = default(E);
            }
        }
    }

    public partial class GlobalEventService : ISingleton, ISingletonAwake
    {
        private static GlobalEventService s_Instance;
        private static Dictionary<Type, List<IGlobalEvent>> s_allGlobalEvents;


        private EventService<Type> m_eventService;
        // private Queue<EventTask> m_eventQueue;

        public bool IsDisposed { get; private set; }

        public void Register()
        {
            if (s_Instance != null)
                throw new Exception($"singleton register twice!");

            s_Instance = this;
            // m_eventQueue = new Queue<EventTask>();
        }

        public void Awake()
        {
            m_eventService = new EventService<Type>();
        }

        public void Dispose()
        {
            if (this.IsDisposed)
                return;

            this.IsDisposed = true;
            s_Instance = null;
        }

        // public void Update()
        // {
        //     int count = m_eventQueue.Count;
        //     while (count-- > 0)
        //     {
        //         var evtTask = m_eventQueue.Dequeue();
        //         evtTask.Invoke(m_eventService);
        //         evtTask.Reset();
        //         ObjectPoolService.Recycle(evtTask);
        //     }
        // }

        public void Subscribe<E>(Action<E> handler)
        {
            m_eventService.Subscribe(TypeCache<E>.TYPE, handler);
        }

        public void Unsubscribe<E>(Action<E> handler)
        {
            m_eventService.Unsubscribe(TypeCache<E>.TYPE, handler);
        }

        public void Publish<E>(E e)
        {
            var t = TypeCache<E>.TYPE;
            if (s_allGlobalEvents.TryGetValue(t, out var evts))
            {
                foreach (var evt in evts)
                {
                    (evt as GlobalEvent<E>)?.Invoke(e);
                }
            }

            m_eventService.Publish(TypeCache<E>.TYPE, e);
        }

        // public void Publish<E>(E e) where E : EventArg
        // {
        //     var eventTask = ObjectPoolService.Spawn<EventTask<E>>();
        //     eventTask.e = e;
        //     m_eventQueue.Enqueue(eventTask);
        // }

        public bool Check<E>()
        {
            return m_eventService.Check(TypeCache<E>.TYPE);
        }

        public void Clear()
        {
            m_eventService.Clear();
        }
    }
}