using System;

namespace Atom
{
    /// <summary>
    /// 固定事件
    /// </summary>
    public class EventManager : GameModule
    {
        private static EventManager s_Instance;
        private static EventStation<Type> s_GlobalEventStation;

        public static EventManager Instance
        {
            get { return s_Instance; }
        }

        private static void InitGlobalEventStation(bool force = false)
        {
            if (!force && s_GlobalEventStation != null)
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
                s_GlobalEventStation.RegisterEvent(eventType, eventHandler);
            }
        }

        private EventStation<Type> m_EventStation;

        public override void Init()
        {
            InitGlobalEventStation();
            m_EventStation = new EventStation<Type>();
            if (s_Instance == null)
            {
                s_Instance = this;
            }
        }

        public override void Shutdown()
        {
            base.Shutdown();
            if (s_Instance == this)
            {
                s_Instance = null;
            }
        }

        public bool HasEvent<T>()
        {
            var evtType = TypeCache<T>.TYPE;
            return s_GlobalEventStation.HasEvent(evtType);
        }

        public void RegisterEvent<T>(EventBase evt)
        {
            var evtType = TypeCache<T>.TYPE;
            m_EventStation.RegisterEvent(evtType, evt);
        }

        public void UnRegisterEvent<T>()
        {
            var evtType = TypeCache<T>.TYPE;
            m_EventStation.UnRegisterEvent(evtType);
        }

        public void UnRegisterAllEvents()
        {
            m_EventStation.UnRegisterAllEvents();
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