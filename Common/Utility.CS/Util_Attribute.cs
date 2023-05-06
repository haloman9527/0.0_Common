#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
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

namespace CZToolKit.Common
{
    public static class Util_Attribute
    {
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
            return TryGetFieldAttribute(Util_Reflection.GetField(type, fieldName), inherit, out attribute);
        }

        public static IEnumerable<T> GetFieldAttributes<T>(Type type, string fieldName, bool inherit)where T : Attribute
        {
            return GetFieldAttributes<T>(Util_Reflection.GetField(type, fieldName), inherit);
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
            return TryGetMethodAttribute(Util_Reflection.GetMethod(type, methodName), inherit, out attribute);
        }
        
        public static IEnumerable<T> GetMethodAttributes<T>(Type type, string methodName, bool inherit) where T : Attribute
        {
            return GetMethodAttributes<T>(Util_Reflection.GetMethod(type, methodName), inherit);
        }
    }
}