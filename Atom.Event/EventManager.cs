using System;

namespace Atom
{
    /// <summary>
    /// 固定事件
    /// </summary>
    public class EventManager : GameModuleSingleton<EventManager>
    {
        private static EventStation<Type> s_GlobalEventStation;

        private EventStation<Type> m_EventStation;

        protected override void OnInit()
        {
            if (s_GlobalEventStation != null)
                return;

            s_GlobalEventStation = new EventStation<Type>();
            foreach (var type in TypesCache.GetTypesDerivedFrom<GlobalEventBase>())
            {
                if (type.IsAbstract)
                    continue;

                var eventHandler = Activator.CreateInstance(type) as GlobalEventBase;
                if (eventHandler == null)
                    continue;

                var eventType = eventHandler.EventType;
                s_GlobalEventStation.Register(eventType, eventHandler);
            }
            
            m_EventStation = new EventStation<Type>();
        }

        public bool HasEvent<T>()
        {
            var evtType = TypeCache<T>.TYPE;
            return s_GlobalEventStation.HasEvent(evtType);
        }

        public void Register<T>(EventBase evt)
        {
            var evtType = TypeCache<T>.TYPE;
            m_EventStation.Register(evtType, evt);
        }

        public void Unregister<T>()
        {
            var evtType = TypeCache<T>.TYPE;
            m_EventStation.Unregister(evtType);
        }

        public void UnregisterAll()
        {
            m_EventStation.UnregisterAll();
        }

        public void Subscribe<T>(Action<T> handler)
        {
            var evtType = TypeCache<T>.TYPE;
            m_EventStation.Subscribe(evtType, handler);
        }

        public void Unsubscribe<T>(Action<T> handler)
        {
            var evtType = TypeCache<T>.TYPE;
            if (m_EventStation.HasEvent(evtType))
                m_EventStation.Unsubscribe(evtType, handler);
        }

        public void Publish<T>(T evt)
        {
            var evtType = TypeCache<T>.TYPE;
            if (s_GlobalEventStation.HasEvent(evtType))
                s_GlobalEventStation?.Publish(evtType, evt);
            if (m_EventStation.HasEvent(evtType))
                m_EventStation.Publish(evtType, evt);
        }
    }
}