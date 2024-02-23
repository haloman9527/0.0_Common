using System;
using System.Collections.Generic;

namespace CZToolKit
{
    public static class Systems
    {
        private class OneTypeSystems
        {
            public Dictionary<Type, List<ISystem>> originSystems = new Dictionary<Type, List<ISystem>>();
            public Dictionary<Type, List<ISystem>> systems = new Dictionary<Type, List<ISystem>>();
        }

        private static bool s_Initialized;
        private static Dictionary<Type, OneTypeSystems> s_Systems;

        static Systems()
        {
            Init(true);
        }

        public static void Init(bool force = false)
        {
            if (!force && s_Initialized)
            {
                return;
            }

            var systemTypes = Util_TypeCache.GetTypesDerivedFrom<ISystem>();
            var entityTypes = Util_TypeCache.GetTypesDerivedFrom<Entity>();

            if (s_Systems == null)
            {
                s_Systems = new Dictionary<Type, OneTypeSystems>();
            }
            else
            {
                s_Systems.Clear();
            }

            foreach (var systemType in systemTypes)
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

                if (!s_Systems.TryGetValue(system.EntityType(), out var systems))
                {
                    s_Systems[system.EntityType()] = systems = new OneTypeSystems();
                }

                if (!systems.originSystems.TryGetValue(systemType, out var lst))
                {
                    systems.originSystems[system.SystemType()] = lst = new List<ISystem>();
                }

                lst.Add(system);
            }

            foreach (var entityType in entityTypes)
            {
                var type = entityType;
                while (type != null)
                {
                    if (!s_Systems.TryGetValue(type, out var systems))
                    {
                        type = type.BaseType;
                        continue;
                    }

                    if (!s_Systems.TryGetValue(entityType, out var entitySystems))
                    {
                        s_Systems[entityType] = entitySystems = new OneTypeSystems();
                    }

                    foreach (var pair in systems.originSystems)
                    {
                        if (!entitySystems.systems.TryGetValue(pair.Key, out var lst))
                        {
                            entitySystems.systems[pair.Key] = lst = new List<ISystem>();
                        }

                        lst.InsertRange(0, pair.Value);
                    }

                    type = type.BaseType;
                }
            }

            s_Initialized = true;
        }

        public static List<ISystem> GetSystems(Type entityType, Type systemType)
        {
            if (!s_Systems.TryGetValue(entityType, out var systems))
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

            for (int i = 0; i < systems.Count; i++)
            {
                ((IAwakeSystem)systems[i]).Execute(entity);
            }
        }

        public static void Awake<A>(Entity entity, A a)
        {
            var type = entity.GetType();
            var systems = GetSystems(type, typeof(IAwakeSystem<A>));
            if (systems == null)
                return;

            for (int i = 0; i < systems.Count; i++)
            {
                ((IAwakeSystem<A>)systems[i]).Execute(entity, a);
            }
        }

        public static void Awake<A, B>(Entity entity, A a, B b)
        {
            var type = entity.GetType();
            var systems = GetSystems(type, typeof(IAwakeSystem<A, B>));
            if (systems == null)
                return;

            for (int i = 0; i < systems.Count; i++)
            {
                ((IAwakeSystem<A, B>)systems[i]).Execute(entity, a, b);
            }
        }

        public static void Awake<A, B, C>(Entity entity, A a, B b, C c)
        {
            var type = entity.GetType();
            var systems = GetSystems(type, typeof(IAwakeSystem<A, B, C>));
            if (systems == null)
                return;

            for (int i = 0; i < systems.Count; i++)
            {
                ((IAwakeSystem<A, B, C>)systems[i]).Execute(entity, a, b, c);
            }
        }

        public static void Awake<A, B, C, D>(Entity entity, A a, B b, C c, D d)
        {
            var type = entity.GetType();
            var systems = GetSystems(type, typeof(IAwakeSystem<A, B, C, D>));
            if (systems == null)
                return;

            for (int i = 0; i < systems.Count; i++)
            {
                ((IAwakeSystem<A, B, C, D>)systems[i]).Execute(entity, a, b, c, d);
            }
        }

        public static void AddComponent(Entity entity, Entity component)
        {
            var type = entity.GetType();
            var systems = GetSystems(type, typeof(IAddComponentSystem));
            if (systems == null)
                return;

            for (int i = 0; i < systems.Count; i++)
            {
                ((IAddComponentSystem)systems[i]).Execute(entity, component);
            }
        }

        public static void OnCreate(Entity entity)
        {
            var type = entity.GetType();
            var systems = GetSystems(type, typeof(IOnCreateSystem));
            if (systems == null)
                return;

            for (int i = 0; i < systems.Count; i++)
            {
                ((IOnCreateSystem)systems[i]).Execute(entity);
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
                if (systems == null)
                {
                    continue;
                }

                for (int i = 0; i < systems.Count; i++)
                {
                    try
                    {
                        ((IFixedUpdateSystem)systems[i]).Execute(component);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
        }

        public static void Update(Queue<int> entitiesQueue)
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

                var systems = GetSystems(component.GetType(), typeof(IUpdateSystem));
                if (systems == null)
                {
                    continue;
                }

                for (int i = 0; i < systems.Count; i++)
                {
                    try
                    {
                        ((IUpdateSystem)systems[i]).Execute(component);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
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
                if (systems == null)
                {
                    continue;
                }

                for (int i = 0; i < systems.Count; i++)
                {
                    try
                    {
                        ((ILateUpdateSystem)systems[i]).Execute(component);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
        }
    }
}