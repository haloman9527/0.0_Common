using System;
using System.Collections.Generic;
using System.Reflection;

namespace CZToolKit.Core
{
    public static class AttributeCache
    {
        /// <summary> 保存类的特性，在编译时重载 </summary>
        private static readonly Dictionary<Type, Attribute[]> TypeAttributes = new Dictionary<Type, Attribute[]>();

        /// <summary> 保存类的字段的特性，在编译时重载 </summary>
        private static readonly Dictionary<Type, Dictionary<string, Attribute[]>> TypeFieldAttributes =
            new Dictionary<Type, Dictionary<string, Attribute[]>>();

        /// <summary> 保存类的方法的特性，在编译时重载 </summary>
        private static readonly Dictionary<Type, Dictionary<string, Attribute[]>> TypeMethodAttributes =
            new Dictionary<Type, Dictionary<string, Attribute[]>>();

        #region Class
        /// <summary> 尝试获取目标类型的目标特性 </summary>
        public static bool TryGetTypeAttribute<AttributeType>(Type classType, out AttributeType attribute)
            where AttributeType : Attribute
        {
            if (TryGetTypeAttributes(classType, out Attribute[] attributes))
            {
                foreach (var tempAttribute in attributes)
                {
                    attribute = tempAttribute as AttributeType;
                    if (attribute != null)
                        return true;
                }
            }

            attribute = null;
            return false;
        }

        /// <summary> 尝试获取目标类型的所有特性 </summary>
        public static bool TryGetTypeAttributes(Type classType, out Attribute[] attributes)
        {
            if (TypeAttributes.TryGetValue(classType, out attributes))
                return attributes == null || attributes.Length > 0;

            attributes = classType.GetCustomAttributes() as Attribute[];
            TypeAttributes[classType] = attributes;
            return attributes == null || attributes.Length > 0;
        }
        #endregion

        #region Field
        /// <summary> 尝试获取目标类型的目标字段的目标特性 </summary>
        public static bool TryGetFieldInfoAttribute<AttributeType>(Type classType, FieldInfo fieldInfo,
            out AttributeType attribute)
            where AttributeType : Attribute
        {
            if (TryGetFieldAttributes(classType, fieldInfo.Name, out Attribute[] attributes))
            {
                for (int i = 0; i < attributes.Length; i++)
                {
                    attribute = attributes[i] as AttributeType;
                    if (attribute != null)
                        return true;
                }
            }

            attribute = null;
            return false;
        }

        /// <summary> 尝试获取目标类型的目标字段的目标特性 </summary>
        public static bool TryGetFieldAttribute<AttributeType>(Type classType, string fieldName,
            out AttributeType attribute)
            where AttributeType : Attribute
        {
            if (TryGetFieldAttributes(classType, fieldName, out Attribute[] attributes))
            {
                for (int i = 0; i < attributes.Length; i++)
                {
                    attribute = attributes[i] as AttributeType;
                    if (attribute != null)
                        return true;
                }
            }

            attribute = null;
            return false;
        }

        /// <summary> 尝试获取目标类型的目标字段的所有特性 </summary>
        public static bool TryGetFieldInfoAttributes<AttributeType>(Type classType, FieldInfo fieldInfo,
            out Attribute[] attributes) where AttributeType : Attribute
        {
            Dictionary<string, Attribute[]> fieldTypes;
            if (TypeFieldAttributes.TryGetValue(classType, out fieldTypes))
            {
                if (fieldTypes.TryGetValue(fieldInfo.Name, out attributes))
                {
                    if (attributes != null && attributes.Length > 0)
                        return true;
                    return false;
                }
            }
            else
                fieldTypes = new Dictionary<string, Attribute[]>();

            attributes = fieldInfo.GetCustomAttributes(typeof(Attribute), true) as Attribute[];
            fieldTypes[fieldInfo.Name] = attributes;
            TypeFieldAttributes[classType] = fieldTypes;
            if (attributes.Length > 0)
                return true;
            return false;
        }

        /// <summary> 尝试获取目标类型的目标字段的所有特性 </summary>
        public static bool TryGetFieldAttributes(Type classType, string fieldName,
            out Attribute[] attributes)
        {
            Dictionary<string, Attribute[]> fieldTypes;
            if (TypeFieldAttributes.TryGetValue(classType, out fieldTypes))
            {
                if (fieldTypes.TryGetValue(fieldName, out attributes))
                {
                    if (attributes != null && attributes.Length > 0)
                        return true;
                    return false;
                }
            }
            else
                fieldTypes = new Dictionary<string, Attribute[]>();

            FieldInfo field = GetFieldInfo(classType, fieldName);
            attributes = field.GetCustomAttributes(typeof(Attribute), true) as Attribute[];
            fieldTypes[fieldName] = attributes;
            TypeFieldAttributes[classType] = fieldTypes;
            if (attributes.Length > 0)
                return true;
            return false;
        }

        /// <summary> 获取字段，包括基类的私有字段 </summary>
        public static FieldInfo GetFieldInfo(Type type, string fieldName)
        {
            // 如果第一次没有找到，那么这个变量可能是基类的私有字段
            FieldInfo field = type.GetField(fieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            // 只搜索基类的私有字段
            while (field == null && (type = type.BaseType) != null)
            {
                field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            }

            return field;
        }
        #endregion

        #region Method
        public static bool TryGetMethodInfoAttribute<AttributeType>(Type classType, MethodInfo methodInfo,
            out AttributeType attribute)
            where AttributeType : Attribute
        {
            if (TryGetMethodAttributes(classType, methodInfo.Name, out Attribute[] attributes))
            {
                for (int i = 0; i < attributes.Length; i++)
                {
                    attribute = attributes[i] as AttributeType;
                    if (attribute != null)
                        return true;
                }
            }

            attribute = null;
            return false;
        }

        /// <summary> 尝试获取目标类型的目标字段的目标特性 </summary>
        public static bool TryGetMethodAttribute<AttributeType>(Type classType, string methodName,
            out AttributeType attribute)
            where AttributeType : Attribute
        {
            if (TryGetMethodAttributes(classType, methodName, out Attribute[] attributes))
            {
                for (int i = 0; i < attributes.Length; i++)
                {
                    attribute = attributes[i] as AttributeType;
                    if (attribute != null)
                        return true;
                }
            }

            attribute = null;
            return false;
        }

        /// <summary> 尝试获取目标类型的目标字段的所有特性 </summary>
        public static bool TryGetMethodInfoAttributes(Type classType, MethodInfo methodInfo,
            out Attribute[] attributes)
        {
            Dictionary<string, Attribute[]> methodTypes;
            if (TypeFieldAttributes.TryGetValue(classType, out methodTypes))
            {
                if (methodTypes.TryGetValue(methodInfo.Name, out attributes))
                {
                    if (attributes != null && attributes.Length > 0)
                        return true;
                    return false;
                }
            }
            else
                methodTypes = new Dictionary<string, Attribute[]>();

            attributes = methodInfo.GetCustomAttributes(typeof(Attribute), true) as Attribute[];
            methodTypes[methodInfo.Name] = attributes;
            TypeFieldAttributes[classType] = methodTypes;
            if (attributes.Length > 0)
                return true;
            return false;
        }

        /// <summary> 尝试获取目标类型的目标字段的所有特性 </summary>
        public static bool TryGetMethodAttributes(Type _classType, string _methodName,
            out Attribute[] attributes)
        {
            Dictionary<string, Attribute[]> methodTypes;
            if (TypeMethodAttributes.TryGetValue(_classType, out methodTypes))
            {
                if (methodTypes.TryGetValue(_methodName, out attributes))
                {
                    if (attributes != null && attributes.Length > 0)
                        return true;
                    return false;
                }
            }
            else
                methodTypes = new Dictionary<string, Attribute[]>();

            MethodInfo field = GetMethodInfo(_classType, _methodName);
            attributes = field.GetCustomAttributes(typeof(Attribute), true) as Attribute[];
            methodTypes[_methodName] = attributes;
            TypeFieldAttributes[_classType] = methodTypes;
            if (attributes.Length > 0)
                return true;
            return false;
        }

        /// <summary> 获取方法，包括基类的私有方法 </summary>
        public static MethodInfo GetMethodInfo(Type type, string methodName)
        {
            // 如果第一次没有找到，那么这个变量可能是基类的私有字段
            MethodInfo method = type.GetMethod(methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            // 只搜索基类的私有方法
            while (method == null && (type = type.BaseType) != null)
            {
                method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            }

            return method;
        }
        #endregion
    }
}