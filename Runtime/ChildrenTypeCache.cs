using System;
using System.Collections.Generic;
using System.Reflection;

namespace CZToolKit.Core
{
    public static class ChildrenTypeCache
    {
        static readonly Dictionary<Type, IEnumerable<Type>> TypeCache = new Dictionary<Type, IEnumerable<Type>>();

        public static IEnumerable<Type> GetChildrenTypes<T>()
        {
            return GetChildrenTypes(typeof(T));
        }

        public static IEnumerable<Type> GetChildrenTypes(Type baseType)
        {
            if (TypeCache.TryGetValue(baseType, out IEnumerable<Type> childrenTypes))
            {
                foreach (var item in childrenTypes)
                {
                    yield return item;
                }
                yield break;
            }
            childrenTypes = BuildCache(baseType);
            TypeCache[baseType] = childrenTypes;
            foreach (var type in childrenTypes)
            {
                yield return type;
            }
        }

        private static IEnumerable<Type> BuildCache(Type baseType)
        {
            var selfAssembly = Assembly.GetAssembly(baseType);
            if (selfAssembly.FullName.StartsWith("Assembly-CSharp") && !selfAssembly.FullName.Contains("-firstpass"))
            {
                // If is not used as a DLL, check only CSharp (fast)
                foreach (var type in selfAssembly.GetTypes())
                {
                    if (!type.IsAbstract && baseType.IsAssignableFrom(type))
                        yield return type;
                }
            }
            else
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                // Else, check all relevant DDLs (slower)
                // ignore all unity related assemblies
                foreach (Assembly assembly in assemblies)
                {
                    if (assembly.FullName.StartsWith("Unity")) continue;
                    // unity created assemblies always have version 0.0.0
                    if (!assembly.FullName.Contains("Version=0.0.0")) continue;
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type != null && type.IsAbstract && baseType.IsAssignableFrom(type))
                            yield return type;
                    }
                }
            }
        }
    }
}