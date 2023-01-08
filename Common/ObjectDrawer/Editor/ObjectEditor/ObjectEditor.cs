#region 注 释
/***
 *
 *  Title:
 *  ObjectEditor
 *  Description:
 *  绘制一个普通对象，可通过继承自定义绘制方式
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
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

using UnityObject = UnityEngine.Object;

namespace CZToolKit.Common.Editors
{
    public class ObjectEditor
    {
        public object Target
        {
            get;
            private set;
        }
        public UnityObject Owner
        {
            get;
            private set;
        }
        public Editor Editor
        {
            get;
            private set;
        }
        public MonoScript Script
        {
            get;
            private set;
        }
        public SerializedPropertyS SerializedObject
        {
            get;
            private set;
        }

        protected ObjectEditor() { }

        void Initialize(object target)
        {
            Target = target;
            Script = EditorUtilityExtension.FindScriptFromType(Target.GetType());
            SerializedObject = new SerializedPropertyS(Target);
        }

        void Initialize(object target, UnityObject context)
        {
            Target = target;
            Owner = context;
            Script = EditorUtilityExtension.FindScriptFromType(Target.GetType());
            SerializedObject = new SerializedPropertyS(Target);
        }

        void Initialize(object target, UnityObject context, Editor editor)
        {
            Target = target;
            Owner = context;
            Editor = editor;
            Script = EditorUtilityExtension.FindScriptFromType(Target.GetType());
            SerializedObject = new SerializedPropertyS(Target);
        }

        public virtual string GetTitle()
        {
            return string.Empty;
        }

        public virtual void OnEnable() { }

        public virtual void OnHeaderGUI() { }

        public virtual void OnInspectorGUI()
        {
            EditorGUILayoutExtension.PropertyField(SerializedObject);
        }

        public virtual bool HasPreviewGUI()
        {
            return false;
        }

        public virtual GUIContent GetPreviewTitle()
        {
            return null;
        }

        public virtual void OnPreviewSettings() { }

        public virtual void DrawPreview(Rect previewArea) { }

        public virtual void OnPreviewGUI(Rect rect, GUIStyle background) { }

        public virtual void OnInteractivePreviewGUI(Rect rect, GUIStyle background) { }

        public virtual void OnValidate() { }

        public virtual void OnSceneGUI() { }

        public virtual void OnDisable() { }

        public virtual VisualElement CreateInspectorGUI()
        {
            return null;
        }

        #region Static
        static Dictionary<Type, Type> ObjectEditorTypeCache;

        static ObjectEditor()
        {
            BuildCache();
        }

        static void BuildCache()
        {
            ObjectEditorTypeCache = new Dictionary<Type, Type>();

            foreach (var type in TypeCache.GetTypesDerivedFrom<ObjectEditor>())
            {
                foreach (var att in Util_Attribute.GetTypeAttributes(type, true))
                {
                    if (att is CustomObjectEditorAttribute sAtt)
                        ObjectEditorTypeCache[sAtt.type] = type;
                }
            }
        }

        public static Type GetEditorType(Type objectType)
        {
            if (ObjectEditorTypeCache.TryGetValue(objectType, out Type editorType))
                return editorType;
            if (objectType.BaseType != null)
                return GetEditorType(objectType.BaseType);
            return null;
        }

        public static ObjectEditor CreateEditor(object target)
        {
            var editorType = GetEditorType(target.GetType());
            if (editorType  == null)
                editorType = typeof(ObjectEditor);
            ObjectEditor objectEditor = Activator.CreateInstance(editorType, true) as ObjectEditor;
            if (objectEditor == null)
                return null;
            objectEditor.Initialize(target);
            return objectEditor;
        }

        public static ObjectEditor CreateEditor(object target, UnityObject context)
        {
            var editorType = GetEditorType(target.GetType());
            if (editorType == null)
                editorType = typeof(ObjectEditor);
            ObjectEditor objectEditor = Activator.CreateInstance(editorType, true) as ObjectEditor;
            if (objectEditor == null)
                return null;
            objectEditor.Initialize(target, context);
            return objectEditor;
        }

        public static ObjectEditor CreateEditor(object target, UnityObject context, Editor editor)
        {
            var editorType = GetEditorType(target.GetType());
            if (editorType == null)
                editorType = typeof(ObjectEditor);
            ObjectEditor objectEditor = Activator.CreateInstance(editorType, true) as ObjectEditor;
            if (objectEditor == null)
                return null;
            objectEditor.Initialize(target, context, editor);
            return objectEditor;
        }

        public static ObjectEditor CreateEditor(object target, Type editorType)
        {
            ObjectEditor objectEditor = Activator.CreateInstance(editorType, true) as ObjectEditor;
            if (objectEditor == null)
                return null;
            objectEditor.Initialize(target);
            return objectEditor;
        }

        public static ObjectEditor CreateEditor(object target, UnityObject context, Type editorType)
        {
            ObjectEditor objectEditor = Activator.CreateInstance(editorType, true) as ObjectEditor;
            if (objectEditor == null)
                return null;
            objectEditor.Initialize(target, context);
            return objectEditor;
        }

        public static ObjectEditor CreateEditor(object target, UnityObject context, Editor editor, Type editorType)
        {
            ObjectEditor objectEditor = Activator.CreateInstance(editorType, true) as ObjectEditor;
            if (objectEditor == null)
                return null;
            objectEditor.Initialize(target, context, editor);
            return objectEditor;
        }

        public static bool HasEditor(object target)
        {
            return HasEditor(target.GetType());
        }

        public static bool HasEditor(Type targetType)
        {
            if (ObjectEditorTypeCache.TryGetValue(targetType, out Type editorType))
                return true;
            if (targetType.BaseType != null)
                return HasEditor(targetType.BaseType);
            return false;

        }

        public static void DrawObjectInInspector(object target, UnityObject context = null)
        {
            if (target is UnityObject)
                Selection.activeObject = target as UnityObject;
            else
                DrawObjectInInspector("Inspector", target, context);
        }

        public static void DrawObjectInInspector(string title, object target, UnityObject context = null)
        {
            var instance = ScriptableObject.CreateInstance<ObjectInspector>();
            instance.name = title;
            instance.Initialize(target, context);
            Selection.activeObject = instance;
        }
        #endregion
    }

    public class CustomObjectEditorAttribute : Attribute
    {
        public readonly Type type;

        public CustomObjectEditorAttribute(Type type)
        {
            this.type = type;
        }
    }
}
#endif