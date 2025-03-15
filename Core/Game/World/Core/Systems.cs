using System;
using System.Collections.Generic;

namespace Atom
{
    public static class Systems
    {
        public class OneTypeSystems
        {
            public Dictionary<Type, List<ISystem>> nodeOriginSystems = new Dictionary<Type, List<ISystem>>();
            public Dictionary<Type, List<ISystem>> nodeSystems = new Dictionary<Type, List<ISystem>>();
            public List<Type> systemTypes = new List<Type>();
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

            var systemTypes = TypesCache.GetTypesDerivedFrom<ISystem>();
            var nodeTypes = TypesCache.GetTypesDerivedFrom<Node>();

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

        public static IReadOnlyList<ISystem> GetSystems(Type nodeType, Type systemType)
        {
            if (!s_Systems.TryGetValue(nodeType, out var systems))
                return ArraySegment<ISystem>.Empty;

            if (!systems.nodeSystems.TryGetValue(systemType, out var lst))
                return ArraySegment<ISystem>.Empty;

            return lst;
        }

        public static OneTypeSystems GetOneTypeSystems(Type nodeType)
        {
            s_Systems.TryGetValue(nodeType, out var systems);
            return systems;
        }

        public static void Awake(Node n)
        {
            Invoke<IAwakeSystem>(n);
        }

        public static void Awake<TArg>(Node n, TArg arg)
        {
            Invoke<IAwakeSystem<TArg>, TArg>(n, arg);
        }

        public static void AddComponent(Node n, Node component)
        {
            Invoke<IAddComponentSystem, Node>(n, component);
        }

        public static void Destroy(Node n)
        {
            Invoke<IDestroySystem>(n);
        }

        public static void Invoke<S>(Node n) where S : ISystem_E
        {
            var type = n.GetType();
            var systems = GetSystems(type, TypeCache<S>.TYPE);

            for (int i = 0; i < systems.Count; i++)
            {
                ((S)systems[i]).Execute(n);
            }
        }

        public static void Invoke<S, TArg>(Node n, TArg arg) where S : ISystem_EA<TArg>
        {
            var type = n.GetType();
            var systems = GetSystems(type, TypeCache<S>.TYPE);

            for (int i = 0; i < systems.Count; i++)
            {
                ((S)systems[i]).Execute(n, arg);
            }
        }
    }
}