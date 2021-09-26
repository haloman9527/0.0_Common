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
using System.Linq;
using System.Reflection;

namespace CZToolKit.Core
{
    public static partial class Utility_Reflection
    {
        static readonly Dictionary<string, Assembly> AssemblyCache = new Dictionary<string, Assembly>();
        static readonly Dictionary<string, Type> FullNameTypeCache = new Dictionary<string, Type>();
        static readonly List<Type> AllTypeCache = new List<Type>();
        static readonly Dictionary<Type, IEnumerable<Type>> ChildrenTypeCache = new Dictionary<Type, IEnumerable<Type>>();

        static Utility_Reflection()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName.StartsWith("Unity")) continue;
                if (!assembly.FullName.Contains("Version=0.0.0")) continue;
                AssemblyCache[assembly.FullName] = assembly;
                AllTypeCache.AddRange(assembly.GetTypes());
            }
        }

        public static IEnumerable<Type> GetChildrenTypes<T>()
        {
            return GetChildrenTypes(typeof(T));
        }

        public static IEnumerable<Type> GetChildrenTypes(Type baseType)
        {
            if (ChildrenTypeCache.TryGetValue(baseType, out IEnumerable<Type> childrenTypes))
            {
                foreach (var item in childrenTypes)
                {
                    yield return item;
                }
                yield break;
            }

            ChildrenTypeCache[baseType] = childrenTypes = BuildCache(baseType);
            foreach (var type in childrenTypes)
            {
                yield return type;
            }
        }

        private static IEnumerable<Type> BuildCache(Type _baseType)
        {
            foreach (var type in AllTypeCache)
            {
                if (_baseType.IsAssignableFrom(type))
                    yield return type;
            }
        }

        public static Assembly LoadAssembly(string _assemblyString)
        {
            Assembly assembly;
            if (!AssemblyCache.TryGetValue(_assemblyString, out assembly))
                AssemblyCache[_assemblyString] = assembly = Assembly.Load(_assemblyString);
            return assembly;
        }

        public static Type GetType(string _fullName, string _assemblyString)
        {
            Type type;
            if (FullNameTypeCache.TryGetValue(_fullName, out type))
                return type;
            Assembly assembly = LoadAssembly(_assemblyString);
            if (assembly == null) return null;
            foreach (var tempType in assembly.GetTypes())
            {
                FullNameTypeCache[tempType.FullName] = tempType;
            }
            if (FullNameTypeCache.TryGetValue(_fullName, out type))
                return type;
            return null;
        }

        #region GetMemberInfo
        static Dictionary<Type, List<FieldInfo>> TypeFieldInfoCache = new Dictionary<Type, List<FieldInfo>>();

        public static IEnumerable<FieldInfo> GetFieldInfos(Type _type)
        {
            Type baseType = _type.BaseType;
            if (baseType != null)
            {
                foreach (var f in GetFieldInfos(baseType))
                {
                    yield return f;
                }
            }

            if (!TypeFieldInfoCache.TryGetValue(_type, out List<FieldInfo> fieldInfos))
            {
                TypeFieldInfoCache[_type] = fieldInfos = new List<FieldInfo>(_type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly));
            }

            foreach (var f in fieldInfos)
            {
                yield return f;
            }
        }

        /// <summary> 获取字段，包括基类的私有字段 </summary>
        public static FieldInfo GetFieldInfo(Type _type, string _fieldName)
        {
            return GetFieldInfos(_type).FirstOrDefault(f => f.Name == _fieldName);
        }

        static Dictionary<Type, List<PropertyInfo>> TypePropertyInfoCache = new Dictionary<Type, List<PropertyInfo>>();

        public static IEnumerable<PropertyInfo> GetPropertyInfos(Type _type)
        {
            Type baseType = _type.BaseType;
            if (baseType != null)
            {
                foreach (var p in GetPropertyInfos(baseType))
                {
                    yield return p;
                }
            }

            if (!TypePropertyInfoCache.TryGetValue(_type, out List<PropertyInfo> propertyInfos))
            {
                TypePropertyInfoCache[_type] = propertyInfos = new List<PropertyInfo>(_type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly));
            }

            foreach (var p in propertyInfos)
            {
                yield return p;
            }
        }

        /// <summary> 获取字段，包括基类的私有字段 </summary>
        public static PropertyInfo GetPropertyInfo(Type _type, string _propertyName)
        {
            return GetPropertyInfos(_type).FirstOrDefault(f => f.Name == _propertyName);
        }

        static Dictionary<Type, List<MethodInfo>> TypeMethodInfoCache = new Dictionary<Type, List<MethodInfo>>();

        public static IEnumerable<MethodInfo> GetMethodInfos(Type _type)
        {
            Type baseType = _type.BaseType;
            if (baseType != null)
            {
                foreach (var m in GetMethodInfos(baseType))
                {
                    yield return m;
                }
            }

            if (!TypeMethodInfoCache.TryGetValue(_type, out List<MethodInfo> methodInfos))
            {
                TypeMethodInfoCache[_type] = methodInfos = new List<MethodInfo>(_type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly));
            }

            foreach (var m in methodInfos)
            {
                yield return m;
            }
        }

        /// <summary> 获取方法，包括基类的私有方法 </summary>
        public static MethodInfo GetMethodInfo(Type _type, string _methodName)
        {
            return GetMethodInfos(_type).FirstOrDefault(t => t.Name == _methodName);
        }
        #endregion
    }
}