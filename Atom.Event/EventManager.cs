using System;

namespace Atom
{
    /// <summary>
    /// 固定事件
    /// </summary>
    public class EventManager : Singleton<EventManager>, ISingletonAwake
    {
        private static EventStation<Type> s_GlobalEventStation;
        private static EventStation<Type> s_EventStation;

        static EventManager()
        {
            s_EventStation = new EventStation<Type>();
        }

        public static void InitGlobalEventStation(bool force = false)
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

        void ISingletonAwake.Awake()
        {
            InitGlobalEventStation();
        }

        public bool HasEvent<T>()
        {
            var evtType = TypeCache<T>.TYPE;
            return s_GlobalEventStation.HasEvent(evtType);
        }

        public void RegisterEvent<T>(EventBase evt)
        {
            var evtType = TypeCache<T>.TYPE;
            s_EventStation.RegisterEvent(evtType, evt);
        }

        public void UnRegisterEvent<T>()
        {
            var evtType = TypeCache<T>.TYPE;
            s_EventStation.UnRegisterEvent(evtType);
        }

        public void UnRegisterAllEvents()
        {
            s_EventStation.UnRegisterAllEvents();
        }

        public void Subscribe<T>(Action<T> handler)
        {
            var evtType = TypeCache<T>.TYPE;
            s_EventStation.Subscribe(evtType, handler);
        }

        public void Unsubscribe<T>(Action<T> handler)
        {
            var evtType = TypeCache<T>.TYPE;
            s_EventStation.Unsubscribe(evtType, handler);
        }

        public void Publish<T>(T evt)
        {
            var evtType = TypeCache<T>.TYPE;
            s_GlobalEventStation?.Publish(evtType, evt);
            s_EventStation.Publish(evtType, evt);
        }
    }
}