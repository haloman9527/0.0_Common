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
 *  Github: https://github.com/haloman9527
 *  Blog: https://www.mindgear.net/
 *
 */

#endregion

using System;
using System.Collections.Generic;
using System.Reflection;

namespace CZToolKit
{
    public static class Util_TypeCache
    {
        private static bool s_Initialized;
        private static List<Type> s_AllTypes;

        public static IReadOnlyList<Type> AllTypes
        {
            get { return s_AllTypes; }
        }

        static Util_TypeCache()
        {
            Init(true);
        }

        public static void Init(bool force)
        {
            if (!force && s_Initialized)
                return;

            if (s_AllTypes == null)
                s_AllTypes = new List<Type>(512);
            else
                s_AllTypes.Clear();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName.StartsWith("UnityEngine.CoreModule")) continue;
                if (!assembly.FullName.Contains("Version=0.0.0")) continue;
                s_AllTypes.AddRange(assembly.GetTypes());
            }

            s_Initialized = true;
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
            return GetTypesWithAttribute(typeof(T), inherit);
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
                foreach (var method in type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
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