using System;
using System.Collections.Generic;
using System.Reflection;

namespace CZToolKit.Core
{
    public static class AttributeCache
    {
        #region Class
        /// <summary> 保存类的特性，在编译时重载 </summary>
        private static readonly Dictionary<Type, Attribute[]> TypeAttributes = new Dictionary<Type, Attribute[]>();

        /// <summary> 尝试获取目标类型的目标特性 </summary>
        public static bool TryGetTypeAttribute<AttributeType>(Type _type, out AttributeType _attribute)
            where AttributeType : Attribute
        {
            if (TryGetTypeAttributes(_type, out Attribute[] attributes))
            {
                foreach (var tempAttribute in attributes)
                {
                    _attribute = tempAttribute as AttributeType;
                    if (_attribute != null)
                        return true;
                }
            }

            _attribute = null;
            return false;
        }

        /// <summary> 尝试获取目标类型的所有特性 </summary>
        public static bool TryGetTypeAttributes(Type _type, out Attribute[] _attributes)
        {
            if (TypeAttributes.TryGetValue(_type, out _attributes))
                return _attributes == null || _attributes.Length > 0;

            _attributes = _type.GetCustomAttributes() as Attribute[];
            TypeAttributes[_type] = _attributes;
            return _attributes == null || _attributes.Length > 0;
        }
        #endregion

        #region Field
        /// <summary> 保存类的字段的特性，在编译时重载 </summary>
        private static readonly Dictionary<Type, Dictionary<string, Attribute[]>> TypeFieldAttributes =
            new Dictionary<Type, Dictionary<string, Attribute[]>>();

        /// <summary> 尝试获取目标类型的目标字段的目标特性 </summary>
        public static bool TryGetFieldInfoAttribute<AttributeType>(Type _type, FieldInfo _fieldInfo,
            out AttributeType _attribute)
            where AttributeType : Attribute
        {
            if (TryGetFieldAttributes(_type, _fieldInfo.Name, out Attribute[] attributes))
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

        /// <summary> 尝试获取目标类型的目标字段的目标特性 </summary>
        public static bool TryGetFieldAttribute<AttributeType>(Type _type, string _fieldName,
            out AttributeType _attribute)
            where AttributeType : Attribute
        {
            if (TryGetFieldAttributes(_type, _fieldName, out Attribute[] attributes))
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

        /// <summary> 尝试获取目标类型的目标字段的所有特性 </summary>
        public static bool TryGetFieldInfoAttributes<AttributeType>(Type _type, FieldInfo _fieldInfo,
            out Attribute[] _attributes) where AttributeType : Attribute
        {
            Dictionary<string, Attribute[]> fieldTypes;
            if (TypeFieldAttributes.TryGetValue(_type, out fieldTypes))
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
            TypeFieldAttributes[_type] = fieldTypes;
            if (_attributes.Length > 0)
                return true;
            return false;
        }

        /// <summary> 尝试获取目标类型的目标字段的所有特性 </summary>
        public static bool TryGetFieldAttributes(Type _type, string _fieldName,
            out Attribute[] _attributes)
        {
            Dictionary<string, Attribute[]> fieldTypes;
            if (TypeFieldAttributes.TryGetValue(_type, out fieldTypes))
            {
                if (fieldTypes.TryGetValue(_fieldName, out _attributes))
                {
                    if (_attributes != null && _attributes.Length > 0)
                        return true;
                    return false;
                }
            }
            else
                fieldTypes = new Dictionary<string, Attribute[]>();

            FieldInfo field = GetFieldInfo(_type, _fieldName);
            _attributes = field.GetCustomAttributes(typeof(Attribute), true) as Attribute[];
            fieldTypes[_fieldName] = _attributes;
            TypeFieldAttributes[_type] = fieldTypes;
            if (_attributes.Length > 0)
                return true;
            return false;
        }

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
        #endregion

        #region Method
        /// <summary> 保存类的方法的特性，在编译时重载 </summary>
        private static readonly Dictionary<Type, Dictionary<string, Attribute[]>> TypeMethodAttributes =
            new Dictionary<Type, Dictionary<string, Attribute[]>>();

        public static bool TryGetMethodInfoAttribute<AttributeType>(Type _type, MethodInfo _methodInfo,
            out AttributeType _attribute)
            where AttributeType : Attribute
        {
            if (TryGetMethodAttributes(_type, _methodInfo.Name, out Attribute[] attributes))
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

        /// <summary> 尝试获取目标类型的目标字段的目标特性 </summary>
        public static bool TryGetMethodAttribute<AttributeType>(Type _type, string _methodName,
            out AttributeType _attribute)
            where AttributeType : Attribute
        {
            if (TryGetMethodAttributes(_type, _methodName, out Attribute[] attributes))
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

        /// <summary> 尝试获取目标类型的目标字段的所有特性 </summary>
        public static bool TryGetMethodInfoAttributes(Type _type, MethodInfo _methodInfo,
            out Attribute[] _attributes)
        {
            Dictionary<string, Attribute[]> methodTypes;
            if (TypeFieldAttributes.TryGetValue(_type, out methodTypes))
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
            TypeFieldAttributes[_type] = methodTypes;
            if (_attributes.Length > 0)
                return true;
            return false;
        }

        /// <summary> 尝试获取目标类型的目标字段的所有特性 </summary>
        public static bool TryGetMethodAttributes(Type _type, string _methodName,
            out Attribute[] _attributes)
        {
            Dictionary<string, Attribute[]> methodTypes;
            if (TypeMethodAttributes.TryGetValue(_type, out methodTypes))
            {
                if (methodTypes.TryGetValue(_methodName, out _attributes))
                {
                    if (_attributes != null && _attributes.Length > 0)
                        return true;
                    return false;
                }
            }
            else
                methodTypes = new Dictionary<string, Attribute[]>();

            MethodInfo field = GetMethodInfo(_type, _methodName);
            _attributes = field.GetCustomAttributes(typeof(Attribute), true) as Attribute[];
            methodTypes[_methodName] = _attributes;
            TypeFieldAttributes[_type] = methodTypes;
            if (_attributes.Length > 0)
                return true;
            return false;
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
                method = _type.GetMethod(_methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            }

            return method;
        }
        #endregion
    }
}