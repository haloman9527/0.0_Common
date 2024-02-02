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
    public static class Util_Reflection
    {
        // ----- Members -----

        public static IEnumerable<MemberInfo> GetMemberInfos(Type type, BindingFlags bindingFlags)
        {
            var declaredOnly = (bindingFlags & BindingFlags.DeclaredOnly) != 0;
            bindingFlags &= ~BindingFlags.DeclaredOnly;
            
            foreach (var m in InnerGetMemberInfos(type))
            {
                yield return m;
            }

            IEnumerable<MemberInfo> InnerGetMemberInfos(Type t)
            {
                if (!declaredOnly && t.BaseType != null)
                {
                    foreach (var m in InnerGetMemberInfos(t.BaseType))
                    {
                        yield return m;
                    }
                }

                foreach (var m in t.GetMembers(bindingFlags))
                {
                    yield return m;
                }
            }
        }

        public static IEnumerable<FieldInfo> GetFields(Type type, BindingFlags bindingFlags)
        {
            var declaredOnly = (bindingFlags & BindingFlags.DeclaredOnly) != 0;
            bindingFlags &= ~BindingFlags.DeclaredOnly;
            
            foreach (var f in InnerGetFields(type))
            {
                yield return f;
            }
            
            IEnumerable<FieldInfo> InnerGetFields(Type t)
            {
                if (!declaredOnly && t.BaseType != null)
                {
                    foreach (var f in InnerGetFields(t.BaseType))
                    {
                        yield return f;
                    }
                }

                foreach (var f in t.GetFields(bindingFlags))
                {
                    yield return f;
                }
            }
        }

        public static IEnumerable<PropertyInfo> GetProperties(Type type, BindingFlags bindingFlags)
        {
            var declaredOnly = (bindingFlags & BindingFlags.DeclaredOnly) != 0;
            bindingFlags &= ~BindingFlags.DeclaredOnly;
            
            foreach (var p in InnerGetProperties(type))
            {
                yield return p;
            }
            
            IEnumerable<PropertyInfo> InnerGetProperties(Type t)
            {
                if (!declaredOnly && t.BaseType != null)
                {
                    foreach (var p in InnerGetProperties(t.BaseType))
                    {
                        yield return p;
                    }
                }

                foreach (var p in t.GetProperties(bindingFlags))
                {
                    yield return p;
                }
            }
        }

        public static IEnumerable<MethodInfo> GetMethods(Type type, BindingFlags bindingFlags)
        {
            var declaredOnly = (bindingFlags & BindingFlags.DeclaredOnly) != 0;
            bindingFlags &= ~BindingFlags.DeclaredOnly;
            
            foreach (var m in InnerGetGetMethods(type))
            {
                yield return m;
            }
            
            IEnumerable<MethodInfo> InnerGetGetMethods(Type t)
            {
                if (!declaredOnly && t.BaseType != null)
                {
                    foreach (var m in InnerGetGetMethods(t.BaseType))
                    {
                        yield return m;
                    }
                }

                foreach (var m in t.GetMethods(bindingFlags))
                {
                    yield return m;
                }
            }
        }

        // ----- Attributes -----

        public static bool TryGetTypeAttribute<T>(Type type, bool inherit, out T attribute) where T : Attribute
        {
            attribute = type.GetCustomAttribute<T>(inherit);
            if (attribute != null)
                return true;
            return false;
        }

        public static IEnumerable<T> GetTypeAttributes<T>(Type type, bool inherit) where T : Attribute
        {
            return type.GetCustomAttributes<T>(inherit);
        }

        public static bool TryGetFieldAttribute<T>(FieldInfo fieldInfo, bool inherit, out T attribute) where T : Attribute
        {
            attribute = fieldInfo.GetCustomAttribute<T>(inherit);
            if (attribute != null)
                return true;
            return false;
        }

        public static IEnumerable<T> GetFieldAttributes<T>(FieldInfo fieldInfo, bool inherit) where T : Attribute
        {
            return fieldInfo.GetCustomAttributes<T>(inherit);
        }

        public static bool TryGetFieldAttribute<T>(Type type, string fieldName, bool inherit, out T attribute) where T : Attribute
        {
            return TryGetFieldAttribute(type.GetField(fieldName), inherit, out attribute);
        }

        public static IEnumerable<T> GetFieldAttributes<T>(Type type, string fieldName, bool inherit) where T : Attribute
        {
            return GetFieldAttributes<T>(type.GetField(fieldName), inherit);
        }

        public static bool TryGetMethodAttribute<T>(MethodInfo methodInfo, bool inherit, out T attribute) where T : Attribute
        {
            attribute = methodInfo.GetCustomAttribute<T>(inherit);
            if (attribute != null)
                return true;
            return false;
        }

        public static IEnumerable<T> GetMethodAttributes<T>(MethodInfo methodInfo, bool inherit) where T : Attribute
        {
            return methodInfo.GetCustomAttributes<T>(inherit);
        }

        public static bool TryGetMethodAttribute<T>(Type type, string methodName, bool inherit, out T attribute) where T : Attribute
        {
            return TryGetMethodAttribute(type.GetMethod(methodName), inherit, out attribute);
        }

        public static IEnumerable<T> GetMethodAttributes<T>(Type type, string methodName, bool inherit) where T : Attribute
        {
            return GetMethodAttributes<T>(type.GetMethod(methodName), inherit);
        }
    }
}