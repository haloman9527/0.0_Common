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
    public static partial class Util_Reflection
    {
        static readonly Dictionary<string, Assembly> AssemblyCache = new Dictionary<string, Assembly>();
        static readonly Dictionary<string, Type> FullNameTypeCache = new Dictionary<string, Type>();
        static readonly List<Type> AllTypeCache = new List<Type>();
        static readonly Dictionary<Type, IEnumerable<Type>> ChildrenTypeCache = new Dictionary<Type, IEnumerable<Type>>();

        static Util_Reflection()
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

        public static IEnumerable<Type> GetChildTypes(Type type)
        {
            if (!ChildrenTypeCache.TryGetValue(type, out IEnumerable<Type> childrenTypes))
                ChildrenTypeCache[type] = childrenTypes = BuildCache(type);

            foreach (var tmpType in childrenTypes)
            {
                yield return tmpType;
            }

            IEnumerable<Type> BuildCache(Type baseType)
            {
                foreach (var tmpType in AllTypeCache)
                {
                    if (tmpType != baseType && baseType.IsAssignableFrom(tmpType))
                        yield return tmpType;
                }
            }
        }

        public static Assembly LoadAssembly(string assemblyString)
        {
            Assembly assembly;
            if (!AssemblyCache.TryGetValue(assemblyString, out assembly))
                AssemblyCache[assemblyString] = assembly = Assembly.Load(assemblyString);
            return assembly;
        }

        public static Type GetType(string fullName)
        {
            if (FullNameTypeCache.TryGetValue(fullName, out Type type))
                return type;
            foreach (var tmpType in AllTypeCache)
            {
                if (tmpType.FullName == fullName)
                {
                    FullNameTypeCache[fullName] = type = tmpType;
                    break;
                }
            }
            return type;
        }

        public static Type GetType(string fullName, string assemblyString)
        {
            if (FullNameTypeCache.TryGetValue(fullName, out Type type))
                return type;
            Assembly assembly = LoadAssembly(assemblyString);
            if (assembly == null) return null;
            foreach (var tempType in assembly.GetTypes())
            {
                FullNameTypeCache[tempType.FullName] = tempType;
            }
            if (FullNameTypeCache.TryGetValue(fullName, out type))
                return type;
            return null;
        }

        #region GetMemberInfo
        static Dictionary<Type, List<MemberInfo>> TypeMemberInfoCache = new Dictionary<Type, List<MemberInfo>>();

        public static IEnumerable<MemberInfo> GetMemberInfos(Type type)
        {
            Type baseType = type.BaseType;
            if (baseType != null)
            {
                foreach (var m in GetMemberInfos(baseType))
                {
                    yield return m;
                }
            }

            if (!TypeMemberInfoCache.TryGetValue(type, out List<MemberInfo> memberInfos))
            {
                TypeMemberInfoCache[type] = memberInfos = new List<MemberInfo>(type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly));
            }

            foreach (var m in memberInfos)
            {
                yield return m;
            }
        }

        /// <summary> 获取字段，包括基类的私有字段 </summary>
        public static MemberInfo GetMemberInfo(Type type, string memberName)
        {
            return GetMemberInfos(type).FirstOrDefault(f => f.Name == memberName);
        }

        public static IEnumerable<FieldInfo> GetFieldInfos(Type _type)
        {
            return GetFieldInfos(_type, false);
        }

        public static IEnumerable<FieldInfo> GetFieldInfos(Type type, bool declaredOnly)
        {
            foreach (var member in GetMemberInfos(type))
            {
                if (declaredOnly && member.DeclaringType != type)
                    continue;

                if (member is FieldInfo fieldInfo)
                    yield return fieldInfo;
            }
        }

        /// <summary> 获取字段，包括基类的私有字段 </summary>
        public static FieldInfo GetFieldInfo(Type type, string fieldName)
        {
            return GetFieldInfos(type).FirstOrDefault(f => f.Name == fieldName);
        }

        public static IEnumerable<PropertyInfo> GetPropertyInfos(Type type)
        {
            foreach (var member in GetMemberInfos(type))
            {
                if (member is PropertyInfo propertyInfo)
                    yield return propertyInfo;
            }
        }

        /// <summary> 获取字段，包括基类的私有字段 </summary>
        public static PropertyInfo GetPropertyInfo(Type type, string propertyName)
        {
            return GetPropertyInfos(type).FirstOrDefault(f => f.Name == propertyName);
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
        public static MethodInfo GetMethodInfo(Type type, string methodName)
        {
            return GetMethodInfos(type).FirstOrDefault(t => t.Name == methodName);
        }
        #endregion

        #region Make Delegate With Result
        public static Func<Arg0, TResult> CreateInstanceDelegate<TInstance, Arg0, TResult>(TInstance instance, MethodInfo method)
        {
            return (Func<Arg0, TResult>)method.CreateDelegate(typeof(Func<Arg0, TResult>), instance);
        }

        public static Func<Arg0, Arg1, TResult> CreateInstanceDelegate<TInstance, Arg0, Arg1, TResult>(TInstance instance, MethodInfo method)
        {
            return (Func<Arg0, Arg1, TResult>)method.CreateDelegate(typeof(Func<Arg0, Arg1, TResult>), instance);
        }

        public static Func<Arg0, Arg1, Arg2, TResult> CreateInstanceDelegate<TInstance, Arg0, Arg1, Arg2, TResult>(TInstance instance, MethodInfo method)
        {
            return (Func<Arg0, Arg1, Arg2, TResult>)method.CreateDelegate(typeof(Func<Arg0, Arg1, Arg2, TResult>), instance);
        }

        public static Func<Arg0, Arg1, Arg2, Arg3, TResult> CreateInstanceDelegate<TInstance, Arg0, Arg1, Arg2, Arg3, TResult>(TInstance instance, MethodInfo method)
        {
            return (Func<Arg0, Arg1, Arg2, Arg3, TResult>)method.CreateDelegate(typeof(Func<Arg0, Arg1, Arg2, Arg3, TResult>), instance);
        }
        #endregion
    }
}