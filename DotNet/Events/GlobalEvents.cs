using System;
using System.Collections.Generic;

namespace Moyo
{
    public interface IGlobalEvent
    {
        public Type EventType { get; }
    }

    public abstract class GlobalEvent<A> : IGlobalEvent where A : struct
    {
        public Type EventType => typeof(A);

        public abstract void Invoke(A arg);
    }

    public partial class GlobalEvents
    {
        private static bool s_Initialized;
        private static Dictionary<Type, List<IGlobalEvent>> s_AllGlobalEvents;

        static GlobalEvents()
        {
            Init();
        }

        public static void Init(bool force = false)
        {
            if (!force && s_Initialized)
            {
                return;
            }

            if (s_AllGlobalEvents == null)
            {
                s_AllGlobalEvents = new Dictionary<Type, List<IGlobalEvent>>(128);
            }
            else
            {
                s_AllGlobalEvents.Clear();
            }

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
                if (!s_AllGlobalEvents.TryGetValue(evtType, out var evts))
                {
                    s_AllGlobalEvents[evtType] = evts = new List<IGlobalEvent>();
                }

                evts.Add(evt);
            }

            s_Initialized = true;
        }

        public static void Publish<A>(A arg) where A : struct
        {
            var evtType = typeof(A);
            if (!s_AllGlobalEvents.TryGetValue(evtType, out var evts))
            {
                return;
            }

            foreach (var evt in evts)
            {
                (evt as GlobalEvent<A>)?.Invoke(arg);
            }
        }
    }

    public partial class GlobalEvents : ISingleton, ISingletonUpdate
    {
        private static GlobalEvents s_Instance;

        public static GlobalEvents Instance
        {
            get { return s_Instance; }
        }

        private Events<string> events;

        public bool IsDisposed { get; private set; }

        public void Register()
        {
            if (s_Instance != null)
                throw new Exception($"singleton register twice!");

            s_Instance = this;
            events = new Events<string>();
        }

        public void Dispose()
        {
            if (this.IsDisposed)
                return;

            this.IsDisposed = true;
            s_Instance = null;
        }

        public void Update()
        {
            events.Update();
        }

        public void Subscribe<TArg>(string id, Action<TArg> handler) where TArg : EventArg
        {
            events.Subscribe(id, handler);
        }

        public void Unsubscribe<TArg>(string id, Action<TArg> handler) where TArg : EventArg
        {
            events.Unsubscribe(id, handler);
        }

        public void Publish<TArg>(string id, TArg e) where TArg : EventArg
        {
            events.Publish(id, e);
        }

        public void Dispatch<TArg>(string id, TArg e) where TArg : EventArg
        {
            events.Dispatch(id, e);
        }

        public bool Check(string id)
        {
            return events.Check(id);
        }

        public void Clear()
        {
            events.Clear();
        }
    }
}