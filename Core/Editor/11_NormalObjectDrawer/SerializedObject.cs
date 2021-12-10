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
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityObject = UnityEngine.Object;

namespace CZToolKit.Core.Editors
{
    public class SerializedObjectS
    {
        public readonly SerializedPropertyS entry;

        public SerializedObjectS(object target)
        {
            entry = new SerializedPropertyS(target);
        }

        public IEnumerable<SerializedPropertyS> GetIterator()
        {
            foreach (var property in entry.GetIterator())
            {
                yield return property;
            }
        }
    }

    public class SerializedPropertyS
    {
        // 直接对象
        private object target;

        // 上下文和FieldInfo
        private object context;
        private FieldInfo fieldInfo;

        private Dictionary<string, SerializedPropertyS> childrens;

        /// <summary> 是否展开 </summary>
        public bool isExpanded;
        public readonly Type propertyType;
        public readonly GUIContent niceName;
        public readonly bool isArray;

        internal FieldInfo FieldInfo { get { return fieldInfo; } }
        internal object Context { get { return context; } }
        public bool HasChildren { get { return HasChildrenInternal(); } }

        public SerializedPropertyS(object target)
        {
            if (target == null)
                throw new NullReferenceException($"{target}不能为空！");

            this.target = target;
            this.niceName = GUIHelper.TextContent(target.GetType().Name);
            this.propertyType = target.GetType();
            this.isArray = typeof(IList).IsAssignableFrom(propertyType);
        }

        internal SerializedPropertyS(FieldInfo fieldInfo, object context)
        {
            this.fieldInfo = fieldInfo;
            this.context = context;
            this.target = fieldInfo.GetValue(context);
            this.propertyType = fieldInfo.FieldType;
            this.niceName = GUIHelper.TextContent(ObjectNames.NicifyVariableName(fieldInfo.Name));
            this.isArray = typeof(IList).IsAssignableFrom(propertyType);

            if (!HasChildren)
            {
                childrens = new Dictionary<string, SerializedPropertyS>();
                if (!typeof(UnityObject).IsAssignableFrom(propertyType))
                    fieldInfo.SetValue(context, EditorGUIExtension.CreateInstance(propertyType));
            }
        }

        void InternalCreate()
        {
            if (target == null)
            {
                target = EditorGUIExtension.CreateInstance(propertyType);
                fieldInfo.SetValue(context, target);
            }

            childrens = new Dictionary<string, SerializedPropertyS>();
            foreach (var fieldInfo in Util_Reflection.GetFieldInfos(propertyType))
            {
                if (fieldInfo.Name.StartsWith("<"))
                    continue;
                // public 修饰符
                if (!fieldInfo.IsPublic)
                {
                    // 如果不带有SerializeField特性
                    if (!Util_Attribute.TryGetFieldAttribute<SerializeField>(fieldInfo, out var serializeField))
                    {
                        continue;
                    }
                    // 若带有NonSerialized特性
                    if (Util_Attribute.TryGetFieldAttribute<NonSerializedAttribute>(fieldInfo, out var nonSerialized))
                    {
                        continue;
                    }
                    // 若带有HideInInspector特性
                    if (Util_Attribute.TryGetFieldAttribute<HideInInspector>(fieldInfo, out var hideInInspector))
                    {
                        continue;
                    }
                }
                childrens[fieldInfo.Name] = new SerializedPropertyS(fieldInfo, target);
            }
        }

        public IEnumerable<SerializedPropertyS> GetIterator()
        {
            if (childrens == null)
                InternalCreate();
            foreach (var pair in childrens)
            {
                yield return pair.Value;
            }
        }

        bool HasChildrenInternal()
        {
            if (EditorGUIExtension.IsBasicType(propertyType))
                return false;
            if (propertyType.IsClass || (propertyType.IsValueType && !propertyType.IsPrimitive))
            {
                if (!typeof(Delegate).IsAssignableFrom(propertyType) && typeof(object).IsAssignableFrom(propertyType))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
