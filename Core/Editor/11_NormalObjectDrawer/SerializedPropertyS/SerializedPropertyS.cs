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
#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CZToolKit.Core.Editors
{
    public class SerializedPropertyS
    {
        // 直接对象
        private object value;
        private string name;

        private IDictionary<int, SerializedPropertyS> childrens;

        // 上下文和FieldInfo
        public readonly SerializedPropertyS root;
        public readonly SerializedPropertyS parent;
        public readonly FieldInfo fieldInfo;
        public readonly PropertyDrawer drawer;

        /// <summary> 是否展开 </summary>
        public bool expanded;
        public readonly Type propertyType;
        public readonly GUIContent niceName;
        public readonly bool isArray;
        public readonly bool isValueType;
        public readonly bool hasChildren;

        private Action onValueChanged;

        public object Value
        {
            get { return value; }
            set
            {
                if (value != null)
                {
                    var tempType = value.GetType();
                    if (!propertyType.IsAssignableFrom(tempType))
                        throw new Exception($"{nameof(value)}({tempType.Name})不是{propertyType.Name}类型");
                }
                this.value = value;
                onValueChanged?.Invoke();
            }
        }
        public string Name
        {
            get { return name; }
            private set { name = value; }
        }
        public bool IsRoot
        {
            get { return parent == null; }
        }

        public SerializedPropertyS(object value, string name)
        {
            if (value == null)
                throw new NullReferenceException($"{nameof(value)}不能为空！");

            this.root = this;
            this.value = value;
            this.name = name;
            this.niceName = GUIHelper.TextContent(ObjectNames.NicifyVariableName(name)); ;
            this.propertyType = value.GetType();
            this.isArray = typeof(IList).IsAssignableFrom(propertyType);
            this.isValueType = propertyType.IsValueType;

            if (EditorGUIExtension.IsBasicType(propertyType))
                hasChildren = false;
            else if (propertyType.IsClass || (propertyType.IsValueType && !propertyType.IsPrimitive))
            {
                if (!typeof(Delegate).IsAssignableFrom(propertyType) && typeof(object).IsAssignableFrom(propertyType))
                    hasChildren = true;
            }

            var editorType = PropertyDrawer.GetEditorType(propertyType);
            var att = (PropertyAttribute)null;
            if (editorType == null && Util_Attribute.TryGetTypeAttribute(propertyType, out att))
                editorType = PropertyDrawer.GetEditorType(propertyType);
            if (editorType != null)
                drawer = PropertyDrawer.CreateEditor(this, att, editorType);
            root = this;
        }

        private SerializedPropertyS(FieldInfo fieldInfo, SerializedPropertyS parent, string name)
        {
            this.root = parent.root;
            this.parent = parent;
            this.fieldInfo = fieldInfo;
            this.value = fieldInfo.GetValue(parent.Value);
            this.propertyType = fieldInfo.FieldType;
            this.name = name;
            this.niceName = GUIHelper.TextContent(ObjectNames.NicifyVariableName(name));
            this.isArray = typeof(IList).IsAssignableFrom(propertyType);
            this.isValueType = propertyType.IsValueType;

            if (EditorGUIExtension.IsBasicType(propertyType))
                hasChildren = false;
            else if (propertyType.IsClass || (propertyType.IsValueType && !propertyType.IsPrimitive))
            {
                if (!typeof(Delegate).IsAssignableFrom(propertyType) && typeof(object).IsAssignableFrom(propertyType))
                    hasChildren = true;
            }

            var editorType = PropertyDrawer.GetEditorType(propertyType);
            var att = (PropertyAttribute)null;
            if (editorType == null && Util_Attribute.TryGetFieldAttribute<PropertyAttribute>(fieldInfo, out att))
                editorType = PropertyDrawer.GetEditorType(att.GetType());
            if (editorType != null)
                drawer = PropertyDrawer.CreateEditor(this, att, editorType);
        }

        public SerializedPropertyS(object value) : this(value, value.GetType().Name) { }

        private SerializedPropertyS(FieldInfo fieldInfo, SerializedPropertyS parent) : this(fieldInfo, parent, string.Empty) { }

        private void Verify()
        {
            if (value == null)
            {
                value = EditorGUIExtension.CreateInstance(propertyType);
                if (fieldInfo != null && parent != null)
                    fieldInfo.SetValue(parent.Value, value);
            }
            if (value == null)
            {
                childrens.Clear();
            }
            else
            {
                if (isArray)
                {
                    if (childrens == null)
                        childrens = new SortedDictionary<int, SerializedPropertyS>();
                    var list = (IList)value;
                    foreach (var key in childrens.Keys.ToArray())
                    {
                        if (key >= list.Count || !childrens[key].value.Equals(list[key]))
                            childrens.Remove(key);
                    }
                    for (int i = 0; i < list.Count; i++)
                    {
                        var index = i;
                        var element = new SerializedPropertyS(list[index], $"Element {index}");
                        element.onValueChanged += () => { list[index] = element.value; };
                        childrens[index] = element;
                    }
                }
                else
                {
                    if (childrens == null)
                        childrens = new Dictionary<int, SerializedPropertyS>();
                    if (hasChildren)
                    {
                        int id = 0;
                        foreach (var childFieldInfo in Util_Reflection.GetFieldInfos(propertyType))
                        {
                            if (childFieldInfo.Name.StartsWith("<"))
                                continue;
                            // public 修饰符
                            if (!childFieldInfo.IsPublic)
                            {
#if UNITY_EDITOR
                                // 如果不带有SerializeField特性
                                if (!Util_Attribute.TryGetFieldAttribute<SerializeField>(childFieldInfo, out var serializeField))
                                {
                                    continue;
                                }
#endif
                                // 若带有NonSerialized特性
                                if (Util_Attribute.TryGetFieldAttribute<NonSerializedAttribute>(childFieldInfo, out var nonSerialized))
                                {
                                    continue;
                                }
                                // 若带有HideInInspector特性
                                if (Util_Attribute.TryGetFieldAttribute<HideInInspector>(childFieldInfo, out var hideInInspector))
                                {
                                    continue;
                                }
                            }
                            if (!childrens.TryGetValue(id, out var child))
                            {
                                childrens[id] = child = new SerializedPropertyS(childFieldInfo, this, childFieldInfo.Name);
                                child.onValueChanged += () => { childFieldInfo.SetValue(value, child.value); };
                                if (isValueType)
                                    child.onValueChanged += () => { Value = value; };
                            }
                            else
                            {
                                var v = childFieldInfo.GetValue(value);
                                if (child.value == null)
                                {
                                    if (v != child.value)
                                        child.value = v;
                                }
                                else
                                {
                                    if (v == null || !v.Equals(child.value))
                                        child.value = v;
                                }
                            }
                            id++;
                        }
                    }
                }
            }
        }

        public IEnumerable<SerializedPropertyS> GetIterator()
        {
            Verify();
            foreach (var pair in childrens)
            {
                yield return pair.Value;
            }
        }
    }
}
#endif