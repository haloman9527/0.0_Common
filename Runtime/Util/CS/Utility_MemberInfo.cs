﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace CZToolKit.Core
{
    public static partial class Utility
    {
        #region GetMemberInfo
        static Dictionary<Type, FieldInfo[]> TypeFieldInfoCache = new Dictionary<Type, FieldInfo[]>();

        /// <summary> 获取字段，包括基类的私有字段 </summary>
        public static FieldInfo GetFieldInfo(Type _type, string _fieldName)
        {
            // 如果第一次没有找到，那么这个变量可能是基类的私有字段
            FieldInfo field = _type.GetField(_fieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            // 只搜索基类的私有字段
            while (field == null && (_type = _type.BaseType) != null)
            {
                field = _type.GetField(_fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            }

            return field;
        }

        public static List<FieldInfo> GetFieldInfos(Type _type)
        {
            List<FieldInfo> fieldInfos =
                new List<FieldInfo>(
                    _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));

            // 获取类包含的所有字段(包含私有)
            while ((_type = _type.BaseType) != null)
            {
                fieldInfos.AddRange(_type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
            }
            return fieldInfos;
        }

        /// <summary> 获取方法，包括基类的私有方法 </summary>
        public static MethodInfo GetMethodInfo(Type _type, string _methodName)
        {
            // 如果第一次没有找到，那么这个变量可能是基类的私有字段
            MethodInfo method = _type.GetMethod(_methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            // 只搜索基类的私有方法
            while (method == null && (_type = _type.BaseType) != null)
            {
                method = _type.GetMethod(_methodName, BindingFlags.NonPublic | BindingFlags.Instance| BindingFlags.DeclaredOnly);
            }

            return method;
        }

        #endregion
    }
}