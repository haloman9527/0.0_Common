#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *      反射操作及缓存
 *  Date:
 *  Version:
 *  Writer: 半只龙虾人
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */
#endregion
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace CZToolKit.Core
{
    public static partial class Util_TypeCache
    {
        static readonly List<Type> allTypes = new List<Type>();

        public static IReadOnlyList<Type> AllTypes
        {
            get { return allTypes; }
        }

        static Util_TypeCache()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName.StartsWith("Unity")) continue;
                if (!assembly.FullName.Contains("Version=0.0.0")) continue;
                allTypes.AddRange(assembly.GetTypes());
            }
        }

        public static IEnumerable<Type> GetTypesWithAttribute(Type attributeType, bool inherit = true)
        {
            foreach (var type in AllTypes)
            {
                if (!type.IsDefined(attributeType, inherit))
                    continue;
                yield return type;
            }
        }

        public static IEnumerable<Type> GetTypesWithAttribute<T>(bool inherit = true) where T : Attribute
        {
            return GetTypesWithAttribute(typeof(T));
        }

        public static IEnumerable<Type> GetTypesDerivedFrom(Type parentType)
        {
            foreach (var type in AllTypes)
            {
                if (type == parentType)
                    continue;
                if (!parentType.IsAssignableFrom(type))
                    continue;
                yield return type;
            }
        }

        public static IEnumerable<Type> GetTypesDerivedFrom<T>()
        {
            return GetTypesDerivedFrom(typeof(T));
        }

        public static IEnumerable<MethodInfo> GetMethodsWithAttribute(Type attributeType, bool inherit = true)
        {
            foreach (var type in AllTypes)
            {
                foreach (var method in type.GetMethods(BindingFlags.DeclaredOnly))
                {
                    if (!method.IsDefined(attributeType, inherit))
                        continue;
                    yield return method;
                }
            }
        }

        public static IEnumerable<MethodInfo> GetMethodsWithAttribute<T>(bool inherit = true) where T : Attribute
        {
            return GetMethodsWithAttribute(typeof(T), inherit);
        }
    }
}