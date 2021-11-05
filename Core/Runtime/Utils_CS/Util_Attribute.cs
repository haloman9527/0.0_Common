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
using System.Linq;
using System.Reflection;

namespace CZToolKit.Core
{
    public static partial class Util_Attribute
    {
        #region Class
        /// <summary> 保存类的特性，在编译时重载 </summary>
        static readonly Dictionary<Type, Attribute[]> TypeAttributes = new Dictionary<Type, Attribute[]>();

        /// <summary> 获取类型的特定特性 </summary>
        public static bool TryGetTypeAttribute<AttributeType>(Type _type, out AttributeType _attribute)
            where AttributeType : Attribute
        {
            foreach (var tempAttribute in GetTypeAttributes(_type))
            {
                _attribute = tempAttribute as AttributeType;
                if (_attribute != null)
                    return true;
            }

            _attribute = null;
            return false;
        }

        /// <summary> 获取类型的所有特性 </summary>
        public static Attribute[] GetTypeAttributes(Type _type, bool _inherit = true)
        {
            if (TypeAttributes.TryGetValue(_type, out var _attributes))
                return _attributes;
            TypeAttributes[_type] = _attributes = CustomAttributeExtensions.GetCustomAttributes(_type, _inherit).ToArray();
            return _attributes;
        }

        /// <summary> 获取类型的所有特性 </summary>
        public static IEnumerable<T> GetTypeAttributes<T>(Type _type, bool _inherit = true) where T : Attribute
        {
            foreach (var attribute in GetTypeAttributes(_type, _inherit))
            {
                if (attribute is T t_attritube)
                    yield return t_attritube;
            }
        }
        #endregion

        #region Field
        /// <summary> 保存字段的特性，在编译时重载 </summary>
        static readonly Dictionary<Type, Dictionary<string, Attribute[]>> TypeFieldAttributes =
            new Dictionary<Type, Dictionary<string, Attribute[]>>();

        /// <summary> 根据<paramref name="fieldInfo"/>获取特定类型特性 </summary>
        public static bool TryGetFieldAttribute<AttributeType>(FieldInfo fieldInfo,
            out AttributeType attribute)
            where AttributeType : Attribute
        {
            attribute = null;
            if (fieldInfo == null) return false;
            foreach (var tmp in GetFieldAttributes(fieldInfo))
            {
                attribute = tmp as AttributeType;
                if (attribute != null)
                    return true;
            }
            return false;
        }

        /// <summary> 根据类型和字段名获取特定类型特性 </summary>
        public static bool TryGetFieldAttribute<AttributeType>(Type type, string fieldName,
            out AttributeType attribute)
            where AttributeType : Attribute
        {
            return TryGetFieldAttribute(Util_Reflection.GetFieldInfo(type, fieldName), out attribute);
        }

        /// <summary> 根据<paramref name="_fieldInfo"/>获取所有特性 </summary>
        public static Attribute[] GetFieldAttributes(FieldInfo _fieldInfo)
        {
            if (!TypeFieldAttributes.TryGetValue(_fieldInfo.DeclaringType, out var fieldTypes))
                TypeFieldAttributes[_fieldInfo.DeclaringType] = fieldTypes = new Dictionary<string, Attribute[]>();

            if (!fieldTypes.TryGetValue(_fieldInfo.Name, out var attributes))
                fieldTypes[_fieldInfo.Name] = attributes = _fieldInfo.GetCustomAttributes(typeof(Attribute), true) as Attribute[];

            return attributes;
        }

        /// <summary> 根据类型和方法名获取所有特性 </summary>
        public static Attribute[] GetFieldAttributes(Type type, string fieldName)
        {
            return GetFieldAttributes(Util_Reflection.GetFieldInfo(type, fieldName));
        }

        /// <summary> 获取类型的所有特性 </summary>
        public static IEnumerable<T> GetFieldAttributes<T>(Type type, string fieldName) where T : Attribute
        {
            foreach (var attribute in GetFieldAttributes(type, fieldName))
            {
                if (attribute is T t_attritube)
                    yield return t_attritube;
            }
        }
        #endregion

        #region Method
        /// <summary> 保存方法的特性，在编译时重载 </summary>
        static readonly Dictionary<Type, Dictionary<string, Attribute[]>> TypeMethodAttributes =
            new Dictionary<Type, Dictionary<string, Attribute[]>>();

        public static bool TryGetMethodAttribute<AttributeType>(MethodInfo _methodInfo,
            out AttributeType _attribute)
            where AttributeType : Attribute
        {
            Attribute[] attributes = GetMethodAttributes(_methodInfo);
            for (int i = 0; i < attributes.Length; i++)
            {
                _attribute = attributes[i] as AttributeType;
                if (_attribute != null)
                    return true;
            }
            _attribute = null;
            return false;
        }

        /// <summary> 根据类型和方法名获取特定类型特性 </summary>
        public static bool TryGetMethodAttribute<AttributeType>(Type _type, string _methodName,
            out AttributeType _attribute)
            where AttributeType : Attribute
        {
            return TryGetMethodAttribute(Util_Reflection.GetMethodInfo(_type, _methodName), out _attribute);
        }

        /// <summary> 根据<paramref name="_methodInfo"/>获取所有特性 </summary>
        public static Attribute[] GetMethodAttributes(MethodInfo _methodInfo)
        {
            if (!TypeMethodAttributes.TryGetValue(_methodInfo.DeclaringType, out var methodTypes))
                TypeMethodAttributes[_methodInfo.DeclaringType] = methodTypes = new Dictionary<string, Attribute[]>();

            if (!methodTypes.TryGetValue(_methodInfo.Name, out var _attributes))
                methodTypes[_methodInfo.Name] = _attributes = _methodInfo.GetCustomAttributes(typeof(Attribute), true) as Attribute[];

            return _attributes;
        }

        /// <summary> 根据类型和方法名获取所有特性 </summary>
        public static Attribute[] GetMethodAttributes(Type _type, string _methodName)
        {
            return GetMethodAttributes(Util_Reflection.GetMethodInfo(_type, _methodName));
        }

        public static IEnumerable<T> GetMethodAttributes<T>(Type _type, string _methodName) where T : Attribute
        {
            Attribute[] _attributes = GetMethodAttributes(_type, _methodName);
            foreach (var attribute in _attributes)
            {
                if (attribute is T t_attritube)
                    yield return t_attritube;
            }
        }
        #endregion
    }
}