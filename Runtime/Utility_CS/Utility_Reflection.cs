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

        public static IEnumerable<Type> GetChildTypes<T>()
        {
            return GetChildTypes(typeof(T));
        }

        public static IEnumerable<Type> GetChildTypes(Type _type)
        {
            if (!ChildrenTypeCache.TryGetValue(_type, out IEnumerable<Type> childrenTypes))
                ChildrenTypeCache[_type] = childrenTypes = BuildCache(_type);

            foreach (var type in childrenTypes)
            {
                yield return type;
            }

            IEnumerable<Type> BuildCache(Type _baseType)
            {
                foreach (var type in AllTypeCache)
                {
                    if (type != _baseType && _baseType.IsAssignableFrom(type))
                        yield return type;
                }
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
        static Dictionary<Type, List<MemberInfo>> TypeMemberInfoCache = new Dictionary<Type, List<MemberInfo>>();

        public static IEnumerable<MemberInfo> GetMemberInfos(Type _type)
        {
            Type baseType = _type.BaseType;
            if (baseType != null)
            {
                foreach (var m in GetMemberInfos(baseType))
                {
                    yield return m;
                }
            }

            if (!TypeMemberInfoCache.TryGetValue(_type, out List<MemberInfo> memberInfos))
            {
                TypeMemberInfoCache[_type] = memberInfos = new List<MemberInfo>(_type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly));
            }

            foreach (var m in memberInfos)
            {
                yield return m;
            }
        }

        /// <summary> 获取字段，包括基类的私有字段 </summary>
        public static MemberInfo GetMemberInfo(Type _type, string _memberName)
        {
            return GetMemberInfos(_type).FirstOrDefault(f => f.Name == _memberName);
        }

        public static IEnumerable<FieldInfo> GetFieldInfos(Type _type)
        {
            foreach (var member in GetMemberInfos(_type))
            {
                if (member is FieldInfo fieldInfo)
                    yield return fieldInfo;
            }
        }

        /// <summary> 获取字段，包括基类的私有字段 </summary>
        public static FieldInfo GetFieldInfo(Type _type, string _fieldName)
        {
            return GetFieldInfos(_type).FirstOrDefault(f => f.Name == _fieldName);
        }

        public static IEnumerable<PropertyInfo> GetPropertyInfos(Type _type)
        {
            foreach (var member in GetMemberInfos(_type))
            {
                if (member is PropertyInfo propertyInfo)
                    yield return propertyInfo;
            }
        }

        /// <summary> 获取字段，包括基类的私有字段 </summary>
        public static PropertyInfo GetPropertyInfo(Type _type, string _propertyName)
        {
            return GetPropertyInfos(_type).FirstOrDefault(f => f.Name == _propertyName);
        }

        public static IEnumerable<MethodInfo> GetMethodInfos(Type _type)
        {
            foreach (var member in GetMemberInfos(_type))
            {
                if (member is MethodInfo methodInfo)
                    yield return methodInfo;
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