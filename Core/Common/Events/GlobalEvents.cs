using System;
using System.Collections.Generic;

namespace CZToolKit
{
    public interface IGlobalEvent
    {
        public Type EventType { get; }
    }

    public abstract class GlobalEvent<A> : IGlobalEvent where A : struct
    {
        public Type EventType => typeof(A);

        public abstract void Handle(A arg);
    }

    public static class GlobalEvents
    {
        private static bool s_Initialized;
        private static Dictionary<Type, List<IGlobalEvent>> s_AllEvents;

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

            if (s_AllEvents == null)
            {
                s_AllEvents = new Dictionary<Type, List<IGlobalEvent>>(128);
            }
            else
            {
                s_AllEvents.Clear();
            }

            foreach (var type in Util_TypeCache.GetTypesDerivedFrom<IGlobalEvent>())
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
                if (!s_AllEvents.TryGetValue(evtType, out var evts))
                {
                    s_AllEvents[evtType] = evts = new List<IGlobalEvent>();
                }

                evts.Add(evt);
            }

            s_Initialized = true;
        }

        public static void Invoke<A>(A arg) where A : struct
        {
            var evtType = typeof(A);
            if (!s_AllEvents.TryGetValue(evtType, out var evts))
            {
                return;
            }

            foreach (var evt in evts)
            {
                (evt as GlobalEvent<A>).Handle(arg);
            }
        }
    }
}