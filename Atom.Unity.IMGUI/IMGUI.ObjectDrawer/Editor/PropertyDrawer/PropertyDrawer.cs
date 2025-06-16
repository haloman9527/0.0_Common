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
 *  Github: https://github.com/haloman9527
 *  Blog: https://www.haloman.net/
 *
 */
#endregion
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace Atom.UnityEditors
{
    public abstract class PropertyDrawer
    {
        public UnityEngine.PropertyAttribute Attribute
        {
            get;
            private set;
        }
        public SerializedPropertyS Property
        {
            get;
            private set;
        }
        public object Target
        {
            get { return Property.Value; }
        }

        protected PropertyDrawer() { }

        public PropertyDrawer(object target)
        {
            Initialize(target, null);
        }

        public PropertyDrawer(object target, UnityEngine.PropertyAttribute attribute)
        {

            Initialize(target, attribute);
        }

        public PropertyDrawer(SerializedPropertyS target)
        {
            Initialize(target, null);
        }

        public PropertyDrawer(SerializedPropertyS target, UnityEngine.PropertyAttribute attribute)
        {
            Initialize(target, attribute);
        }

        void Initialize(object target, UnityEngine.PropertyAttribute attribute)
        {
            Property = new SerializedPropertyS(target);
            Attribute = attribute;
            OnEnable();
        }

        void Initialize(SerializedPropertyS target, UnityEngine.PropertyAttribute attribute)
        {
            Property = target;
            Attribute = attribute;
            OnEnable();
        }

        public virtual void OnEnable() { }

        public virtual void OnGUI(Rect position, GUIContent label)
        {
            EditorGUILayoutExtension.PropertyField(Property);
        }

        public virtual float GetHeight(GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        #region Static
        static Dictionary<Type, Type> ObjectEditorTypeCache;

        static PropertyDrawer()
        {
            BuildCache();
        }

        static void BuildCache()
        {
            ObjectEditorTypeCache = new Dictionary<Type, Type>();

            foreach (var type in TypeCache.GetTypesDerivedFrom<PropertyDrawer>())
            {
                foreach (var attr in ReflectionEx.GetTypeAttributes<CustomPropertyDrawerAttribute>(type, false))
                {
                     ObjectEditorTypeCache[attr.type] = type;
                }
            }
        }

        public static Type GetEditorType(Type type)
        {
            if (ObjectEditorTypeCache.TryGetValue(type, out Type editorType))
                return editorType;
            if (type.BaseType != null)
                return GetEditorType(type.BaseType);
            else
                return null;
        }

        public static PropertyDrawer CreateEditor(object target)
        {
            return CreateEditor(target, (UnityEngine.PropertyAttribute)null);
        }

        public static PropertyDrawer CreateEditor(object target, Type editorType)
        {
            return CreateEditor(target, (UnityEngine.PropertyAttribute)null, editorType);
        }

        public static PropertyDrawer CreateEditor(SerializedPropertyS target, Type editorType)
        {
            return CreateEditor(target, null, editorType);
        }

        public static PropertyDrawer CreateEditor(object target, UnityEngine.PropertyAttribute attribute)
        {
            var editorType = GetEditorType(target.GetType());
            return CreateEditor(target, attribute, editorType);
        }

        public static PropertyDrawer CreateEditor(object target, UnityEngine.PropertyAttribute attribute, Type editorType)
        {
            if (editorType == null)
                return null;
            PropertyDrawer drawer = Activator.CreateInstance(editorType, true) as PropertyDrawer;
            if (drawer != null)
                drawer.Initialize(target, attribute);
            return drawer;
        }

        public static PropertyDrawer CreateEditor(SerializedPropertyS target, UnityEngine.PropertyAttribute attribute, Type editorType)
        {
            PropertyDrawer drawer = Activator.CreateInstance(editorType, true) as PropertyDrawer;
            if (drawer != null)
                drawer.Initialize(target, attribute);
            return drawer;
        }
        #endregion
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    public class CustomPropertyDrawerAttribute : Attribute
    {
        public Type type;

        public CustomPropertyDrawerAttribute(Type type)
        {
            this.type = type;
        }
    }
}
#endif