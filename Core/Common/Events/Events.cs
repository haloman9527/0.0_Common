using System;
using System.Collections.Generic;

namespace CZToolKit
{
    public class Events
    {
        private static bool s_Initialized;
        private static Dictionary<Type, List<IEvent>> s_AllEvents;

        static Events()
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
                s_AllEvents = new Dictionary<Type, List<IEvent>>(128);
            }
            else
            {
                s_AllEvents.Clear();
            }

            foreach (var type in Util_TypeCache.GetTypesDerivedFrom<IEvent>())
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                if (!type.IsClass)
                {
                    continue;
                }

                var evt = Activator.CreateInstance(type) as IEvent;
                if (evt == null)
                {
                    continue;
                }

                var evtType = type.GetInterfaces()[0].GetGenericArguments()[0];
                if (!s_AllEvents.TryGetValue(evtType, out var evts))
                {
                    s_AllEvents[evtType] = evts = new List<IEvent>();
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
                (evt as IEvent<A>).Handle(arg);
            }
        }
    }
}