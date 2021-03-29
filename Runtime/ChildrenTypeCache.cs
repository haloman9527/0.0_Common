using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CZToolKit.Core
{
    public static class ChildrenTypeCache
    {
        static readonly Dictionary<Type, List<Type>> TypeCache = new Dictionary<Type, List<Type>>();

        public static List<Type> GetChildrenTypes<T>()
        {
            return GetChildrenTypes(typeof(T));
        }

        public static List<Type> GetChildrenTypes(Type baseType)
        {
            List<Type> childrenTypes;
            if (TypeCache.TryGetValue(baseType, out childrenTypes))
                return new List<Type>(childrenTypes);

            childrenTypes = new List<Type>();
            
            var selfAssembly = Assembly.GetAssembly(baseType);
            if (selfAssembly.FullName.StartsWith("Assembly-CSharp") && !selfAssembly.FullName.Contains("-firstpass"))
            {
                // If is not used as a DLL, check only CSharp (fast)
                childrenTypes.AddRange(
                    selfAssembly.GetTypes().Where(t => !t.IsAbstract && baseType.IsAssignableFrom(t)));
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
                    childrenTypes.AddRange(assembly.GetTypes().Where(t => !t.IsAbstract && baseType.IsAssignableFrom(t))
                        .ToArray());
                }
            }
            childrenTypes.RemoveAll(a => a == null);
            TypeCache[baseType] = childrenTypes;
            return new List<Type>(childrenTypes);
        }
    }
}