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

        /// <summary> 根据<paramref name="_fieldInfo"/>获取特定类型特性 </summary>
        public static bool TryGetFieldInfoAttribute<AttributeType>(FieldInfo _fieldInfo,
            out AttributeType _attribute)
            where AttributeType : Attribute
        {
            _attribute = null;
            if (_fieldInfo == null) return false;
            if (TryGetFieldInfoAttributes(_fieldInfo, out Attribute[] attributes))
            {
                for (int i = 0; i < attributes.Length; i++)
                {
                    _attribute = attributes[i] as AttributeType;
                    if (_attribute != null)
                        return true;
                }
            }
            return false;
        }

        /// <summary> 根据类型和字段名获取特定类型特性 </summary>
        public static bool TryGetFieldAttribute<AttributeType>(Type _type, string _fieldName,
            out AttributeType _attribute)
            where AttributeType : Attribute
        {
            return TryGetFieldInfoAttribute(Util_Reflection.GetFieldInfo(_type, _fieldName), out _attribute);
        }

        /// <summary> 根据<paramref name="_fieldInfo"/>获取所有特性 </summary>
        public static bool TryGetFieldInfoAttributes(FieldInfo _fieldInfo,
            out Attribute[] _attributes)
        {
            Dictionary<string, Attribute[]> fieldTypes;
            if (TypeFieldAttributes.TryGetValue(_fieldInfo.DeclaringType, out fieldTypes))
            {
                if (fieldTypes.TryGetValue(_fieldInfo.Name, out _attributes))
                {
                    if (_attributes != null && _attributes.Length > 0)
                        return true;
                    return false;
                }
            }
            else
                fieldTypes = new Dictionary<string, Attribute[]>();

            _attributes = _fieldInfo.GetCustomAttributes(typeof(Attribute), true) as Attribute[];
            fieldTypes[_fieldInfo.Name] = _attributes;
            TypeFieldAttributes[_fieldInfo.DeclaringType] = fieldTypes;
            if (_attributes.Length > 0)
                return true;
            return false;
        }

        /// <summary> 根据类型和方法名获取所有特性 </summary>
        public static bool TryGetFieldAttributes(Type _type, string _fieldName,
            out Attribute[] _attributes)
        {
            return TryGetFieldInfoAttributes(Util_Reflection.GetFieldInfo(_type, _fieldName), out _attributes);
        }

        /// <summary> 获取类型的所有特性 </summary>
        public static IEnumerable<T> GetFieldAttributes<T>(Type _type, string _fieldName) where T : Attribute
        {
            if (TryGetFieldAttributes(_type, _fieldName, out Attribute[] _attributes))
            {
                foreach (var attribute in _attributes)
                {
                    if (attribute is T t_attritube)
                        yield return t_attritube;
                }
            }
        }
        #endregion

        #region Method
        /// <summary> 保存方法的特性，在编译时重载 </summary>
        static readonly Dictionary<Type, Dictionary<string, Attribute[]>> TypeMethodAttributes =
            new Dictionary<Type, Dictionary<string, Attribute[]>>();

        public static bool TryGetMethodInfoAttribute<AttributeType>(MethodInfo _methodInfo,
            out AttributeType _attribute)
            where AttributeType : Attribute
        {
            if (TryGetMethodInfoAttributes(_methodInfo, out Attribute[] attributes))
            {
                for (int i = 0; i < attributes.Length; i++)
                {
                    _attribute = attributes[i] as AttributeType;
                    if (_attribute != null)
                        return true;
                }
            }

            _attribute = null;
            return false;
        }

        /// <summary> 根据类型和方法名获取特定类型特性 </summary>
        public static bool TryGetMethodAttribute<AttributeType>(Type _type, string _methodName,
            out AttributeType _attribute)
            where AttributeType : Attribute
        {
            return TryGetMethodInfoAttribute(Util_Reflection.GetMethodInfo(_type, _methodName), out _attribute);
        }

        /// <summary> 根据<paramref name="_methodInfo"/>获取所有特性 </summary>
        public static bool TryGetMethodInfoAttributes(MethodInfo _methodInfo,
            out Attribute[] _attributes)
        {
            Dictionary<string, Attribute[]> methodTypes;
            if (TypeMethodAttributes.TryGetValue(_methodInfo.DeclaringType, out methodTypes))
            {
                if (methodTypes.TryGetValue(_methodInfo.Name, out _attributes))
                {
                    if (_attributes != null && _attributes.Length > 0)
                        return true;
                    return false;
                }
            }
            else
                methodTypes = new Dictionary<string, Attribute[]>();

            _attributes = _methodInfo.GetCustomAttributes(typeof(Attribute), true) as Attribute[];
            methodTypes[_methodInfo.Name] = _attributes;
            TypeMethodAttributes[_methodInfo.DeclaringType] = methodTypes;
            if (_attributes.Length > 0)
                return true;
            return false;
        }

        /// <summary> 根据类型和方法名获取所有特性 </summary>
        public static bool TryGetMethodAttributes(Type _type, string _methodName,
            out Attribute[] _attributes)
        {
            return TryGetMethodInfoAttributes(Util_Reflection.GetMethodInfo(_type, _methodName), out _attributes);
        }

        public static IEnumerable<T> GetMethodAttributes<T>(Type _type, string _methodName) where T : Attribute
        {
            if (TryGetMethodAttributes(_type, _methodName, out Attribute[] _attributes))
            {
                foreach (var attribute in _attributes)
                {
                    if (attribute is T t_attritube)
                        yield return t_attritube;
                }
            }
        }
        #endregion
    }
}