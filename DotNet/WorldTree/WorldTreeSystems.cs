using System;
using System.Collections.Generic;

namespace Moyo
{
    public static class WorldTreeSystems
    {
        public class OneTypeSystems
        {
            public Dictionary<Type, List<ISystem>> nodeOriginSystems = new Dictionary<Type, List<ISystem>>();
            public Dictionary<Type, List<ISystem>> nodeSystems = new Dictionary<Type, List<ISystem>>();
            public List<Type> systemTypes = new List<Type>();
        }

        private static bool s_Initialized;
        private static Dictionary<Type, OneTypeSystems> s_Systems;

        static WorldTreeSystems()
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
            var nodeTypes = Util_TypeCache.GetTypesDerivedFrom<Node>();

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

                if (!s_Systems.TryGetValue(system.NodeType(), out var systems))
                {
                    s_Systems[system.NodeType()] = systems = new OneTypeSystems();
                }

                if (!systems.nodeOriginSystems.TryGetValue(systemType, out var lst))
                {
                    systems.nodeOriginSystems[system.SystemType()] = lst = new List<ISystem>();
                }

                lst.Add(system);
            }

            foreach (var nodeType in nodeTypes)
            {
                var tempNodeType = nodeType;
                while (tempNodeType != null)
                {
                    if (!s_Systems.TryGetValue(tempNodeType, out var systems))
                    {
                        tempNodeType = tempNodeType.BaseType;
                        continue;
                    }

                    if (!s_Systems.TryGetValue(nodeType, out var nodeSystems))
                    {
                        s_Systems[nodeType] = nodeSystems = new OneTypeSystems();
                    }

                    foreach (var pair in systems.nodeOriginSystems)
                    {
                        if (!nodeSystems.nodeSystems.TryGetValue(pair.Key, out var lst))
                        {
                            nodeSystems.nodeSystems[pair.Key] = lst = new List<ISystem>();
                        }

                        lst.InsertRange(0, pair.Value);
                    }

                    tempNodeType = tempNodeType.BaseType;
                }
            }

            s_Initialized = true;
        }

        public static List<ISystem> GetSystems(Type nodeType, Type systemType)
        {
            if (!s_Systems.TryGetValue(nodeType, out var systems))
                return null;

            if (!systems.nodeSystems.TryGetValue(systemType, out var lst))
                return null;

            return lst;
        }

        public static OneTypeSystems GetOneTypeSystems(Type nodeType)
        { 
            OneTypeSystems systems = null;
            s_Systems.TryGetValue(nodeType, out systems);
            return systems;
        }

        public static void Awake(Node entity)
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

        public static void Awake<A>(Node entity, A a)
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

        public static void Awake<A, B>(Node entity, A a, B b)
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

        public static void Awake<A, B, C>(Node entity, A a, B b, C c)
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

        public static void Awake<A, B, C, D>(Node entity, A a, B b, C c, D d)
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

        public static void AddComponent(Node entity, Node component)
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

        public static void Destroy(Node entity)
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
    }
}