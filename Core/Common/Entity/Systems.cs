using System;
using System.Collections.Generic;

namespace CZToolKit.ET
{
    public static class Systems
    {
        private class OneTypeSystems
        {
            public Dictionary<Type, List<ISystem>> systems = new Dictionary<Type, List<ISystem>>();
        }

        private static bool s_Initialized;
        private static Dictionary<Type, OneTypeSystems> s_Systems;

        static Systems()
        {
            Init(true);
        }

        public static void Init(bool force)
        {
            if (!force && s_Initialized)
            {
                return;
            }

            if (s_Systems == null)
            {
                s_Systems = new Dictionary<Type, OneTypeSystems>();
            }
            else
            {
                s_Systems.Clear();
            }

            foreach (var systemType in Util_TypeCache.GetTypesDerivedFrom<ISystem>())
            {
                if (!systemType.IsClass)
                {
                    continue;
                }

                if (systemType.IsAbstract)
                {
                    continue;
                }

                var system = Activator.CreateInstance(systemType) as ISystem;
                if (system == null)
                {
                    continue;
                }

                if (!s_Systems.TryGetValue(system.Type(), out var systems))
                {
                    s_Systems[system.Type()] = systems = new OneTypeSystems();
                }

                if (!systems.systems.TryGetValue(systemType, out var lst))
                {
                    systems.systems[system.SystemType()] = lst = new List<ISystem>();
                }

                lst.Add(system);
            }

            foreach (var pair in s_Systems)
            {
                var type = pair.Key.BaseType;
                while (type != null)
                {
                    if (!s_Systems.TryGetValue(type, out var systems))
                    {
                        break;
                    }

                    foreach (var pair1 in systems.systems)
                    {
                        if (!pair.Value.systems.TryGetValue(pair1.Key, out var lst))
                        {
                            pair.Value.systems[pair1.Key] = lst = new List<ISystem>();
                        }

                        lst.InsertRange(0, pair1.Value);
                    }

                    type = type.BaseType;
                }
            }

            s_Initialized = true;
        }

        public static List<ISystem> GetSystems(Type type, Type systemType)
        {
            if (!s_Systems.TryGetValue(type, out var systems))
                return null;

            if (!systems.systems.TryGetValue(systemType, out var lst))
                return null;

            return lst;
        }

        public static void Awake(Entity entity)
        {
            var type = entity.GetType();
            var systems = GetSystems(type, typeof(IAwakeSystem));
            if (systems == null)
                return;

            foreach (var system in systems)
            {
                ((IAwakeSystem)system).Execute(entity);
            }
        }

        public static void Awake<A>(Entity entity, A a)
        {
            var type = entity.GetType();
            var systems = GetSystems(type, typeof(IAwakeSystem<A>));
            if (systems == null)
                return;

            foreach (var system in systems)
            {
                ((IAwakeSystem<A>)system).Execute(entity, a);
            }
        }

        public static void Awake<A, B>(Entity entity, A a, B b)
        {
            var type = entity.GetType();
            var systems = GetSystems(type, typeof(IAwakeSystem<A, B>));
            if (systems == null)
                return;

            foreach (var system in systems)
            {
                ((IAwakeSystem<A, B>)system).Execute(entity, a, b);
            }
        }

        public static void Destroy(Entity entity)
        {
            var type = entity.GetType();
            var systems = GetSystems(type, typeof(IDestroySystem));
            if (systems == null)
                return;

            for (int i = systems.Count - 1; i >= 0; i--)
            {
                ((IDestroySystem)systems[i]).Execute(entity);
            }
        }

        public static void FixedUpdate(Queue<int> entitiesQueue)
        {
            int count = entitiesQueue.Count;
            while (count-- > 0)
            {
                var instanceId = entitiesQueue.Dequeue();
                var component = Root.Instance.Get(instanceId);
                if (component == null)
                {
                    continue;
                }

                if (component.IsDisposed)
                {
                    continue;
                }

                entitiesQueue.Enqueue(instanceId);
                
                var systems = GetSystems(component.GetType(), typeof(IFixedUpdateSystem));
                if (systems != null)
                {
                    foreach (var system in systems)
                    {
                        try
                        {
                            ((IFixedUpdateSystem)system).Execute(component);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                        }
                    }
                }
            }
        }

        public static void LateUpdate(Queue<int> entitiesQueue)
        {
            int count = entitiesQueue.Count;
            while (count-- > 0)
            {
                var instanceId = entitiesQueue.Dequeue();
                var component = Root.Instance.Get(instanceId);
                if (component == null)
                {
                    continue;
                }

                if (component.IsDisposed)
                {
                    continue;
                }

                entitiesQueue.Enqueue(instanceId);
                var systems = GetSystems(component.GetType(), typeof(ILateUpdateSystem));
                if (systems != null)
                {
                    foreach (var system in systems)
                    {
                        try
                        {
                            ((ILateUpdateSystem)system).Execute(component);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                        }
                    }
                }
            }
        }

        public static void AddComponent(Entity entity, Entity component)
        {
            var type = entity.GetType();
            var systems = GetSystems(type, typeof(IAddComponentSystem));
            if (systems == null)
                return;

            foreach (var system in systems)
            {
                ((IAddComponentSystem)system).Execute(entity, component);
            }
        }
    }
}