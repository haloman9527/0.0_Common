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

namespace CZToolKit.Common
{
    public static class Util_Reflection
    {
        public static IEnumerable<MemberInfo> GetMemberInfos(Type type, BindingFlags bindingFlags, bool declaredOnly)
        {
            foreach (var m in type.GetMembers(bindingFlags))
            {
                yield return m;
            }
            
            if (declaredOnly && type.BaseType != null)
            {
                foreach (var m in GetMemberInfos(type.BaseType, bindingFlags, true))
                {
                    yield return m;
                }
            }
        }

        public static IEnumerable<FieldInfo> GetFields(Type type, BindingFlags bindingFlags, bool declaredOnly)
        {
            foreach (var f in type.GetFields(bindingFlags))
            {
                yield return f;
            }
            
            if (declaredOnly && type.BaseType != null)
            {
                foreach (var f in GetFields(type.BaseType, bindingFlags, true))
                {
                    yield return f;
                }
            }
        }

        public static FieldInfo GetField(Type type, string name)
        {
            return type.GetField(name);
        }

        public static IEnumerable<PropertyInfo> GetProperties(Type type, BindingFlags bindingFlags, bool declaredOnly)
        {
            foreach (var p in type.GetProperties(bindingFlags))
            {
                yield return p;
            }
            
            if (declaredOnly && type.BaseType != null)
            {
                foreach (var p in GetProperties(type.BaseType, bindingFlags, true))
                {
                    yield return p;
                }
            }
        }

        public static PropertyInfo GetPropertyInfo(Type type, string name)
        {
            return GetProperties(type, BindingFlags.Default, true).FirstOrDefault(f => f.Name == name);
        }

        public static IEnumerable<MethodInfo> GetMethods(Type type, BindingFlags bindingFlags, bool declaredOnly)
        {
            foreach (var m in type.GetMethods(bindingFlags))
            {
                yield return m;
            }
            
            if (declaredOnly && type.BaseType != null)
            {
                foreach (var m in GetMethods(type.BaseType, bindingFlags, true))
                {
                    yield return m;
                }
            }
        }

        public static MethodInfo GetMethod(Type type, string name)
        {
            return GetMethods(type, BindingFlags.Default, true).FirstOrDefault(t => t.Name == name);
        }
    }
}