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
        static readonly Dictionary<Type, HashSet<Type>> ChildrenTypeCache = new Dictionary<Type, HashSet<Type>>();

        public static IEnumerable<Type> GetChildTypes<T>(bool inherit = true)
        {
            return GetChildTypes(typeof(T), inherit);
        }

        public static IEnumerable<Type> GetChildTypes(Type baseType, bool inherit = true)
        {
            if (!ChildrenTypeCache.ContainsKey(baseType))
                BuildCache(baseType);
            if (!ChildrenTypeCache.TryGetValue(baseType, out var childrenTypes))
                yield break;
            foreach (var type1 in childrenTypes)
            {
                yield return type1;
                if (inherit)
                {
                    foreach (var type2 in GetChildTypes(type1, inherit))
                    {
                        yield return type2;
                    }
                }
            }
        }

        static void BuildCache(Type parentType)
        {
            
            foreach (var type in Util_TypeCache.AllTypes)
            {
                if (type == parentType)
                    continue;
                if (type.BaseType != parentType)
                    continue;
                if (!ChildrenTypeCache.TryGetValue(parentType, out var types))
                    ChildrenTypeCache[parentType] = types = new HashSet<Type>();
                types.Add(type);
            }
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