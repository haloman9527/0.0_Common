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
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

using UnityObject = UnityEngine.Object;

namespace CZToolKit.Core.Editors
{
    [Serializable]
    public class GenericObject<T>
    {
        public T t;
        public GenericObject(T t)
        {
            this.t = t;
        }
    }

    public class ObjectEditor
    {
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
                        ObjectEditorTypeCache[sAtt.targetType] = type;
                }
            }
        }

        public static Type GetEditorType(Type objectType)
        {
            if (ObjectEditorTypeCache.TryGetValue(objectType, out Type editorType))
                return editorType;
            if (objectType.BaseType != null)
                return GetEditorType(objectType.BaseType);
            else
                return typeof(ObjectEditor);
        }

        public static ObjectEditor CreateEditor(object target)
        {
            ObjectEditor objectEditor = InternalCreateEditor(target);
            if (objectEditor == null) return null;

            objectEditor.Initialize(target);
            return objectEditor;
        }

        public static ObjectEditor CreateEditor(object target, UnityObject context)
        {
            ObjectEditor objectEditor = InternalCreateEditor(target);
            if (objectEditor == null) return null;

            objectEditor.Initialize(target, context);
            return objectEditor;
        }

        public static ObjectEditor CreateEditor(object target, UnityObject context, Editor editor)
        {
            ObjectEditor objectEditor = InternalCreateEditor(target);
            if (objectEditor == null) return null;

            objectEditor.Initialize(target, context, editor);
            return objectEditor;
        }

        static ObjectEditor InternalCreateEditor(object target)
        {
            if (target == null) return null;
            return Activator.CreateInstance(GetEditorType(target.GetType()), true) as ObjectEditor;
        }

        protected IReadOnlyList<FieldInfo> Fields { get; private set; }

        public object Target { get; private set; }
        public UnityObject Owner { get; private set; }
        public Editor Editor { get; private set; }
        public MonoScript Script { get; private set; }

        protected ObjectEditor() { }

        void Initialize(object target)
        {
            Target = target;
            Script = EditorUtilityExtension.FindScriptFromType(Target.GetType());
            Fields = Util_Reflection.GetFieldInfos(Target.GetType()).Where(field => EditorGUILayoutExtension.CanDraw(field)).ToList();
        }

        void Initialize(object target, UnityObject context)
        {
            Owner = context;
            Initialize(target);
        }

        void Initialize(object target, UnityObject context, Editor editor)
        {
            Owner = context;
            Editor = editor;
            Initialize(target);
        }

        public virtual string GetTitle() { return string.Empty; }

        public virtual void OnEnable() { }

        public virtual void OnHeaderGUI() { }

        public virtual void OnInspectorGUI()
        {
            DrawBaseInspector();
        }

        public void DrawBaseInspector()
        {
            if (Editor is ObjectInspectorEditor e)
                e.DrawBaseInspecotrGUI();
        }

        public void DrawDefaultInspector()
        {
            Editor.DrawDefaultInspector();
        }

        public virtual bool HasPreviewGUI() { return false; }

        public virtual GUIContent GetPreviewTitle() { return null; }

        public virtual void OnPreviewSettings() { }

        public virtual void DrawPreview(Rect previewArea) { }

        public virtual void OnPreviewGUI(Rect rect, GUIStyle background) { }

        public virtual void OnInteractivePreviewGUI(Rect rect, GUIStyle background) { }

        public virtual void OnValidate() { }

        public virtual void OnSceneGUI() { }

        public virtual void OnDisable() { }

        public VisualElement CreateInspectorGUI() { return null; }
    }

    public class CustomObjectEditorAttribute : Attribute
    {
        public readonly Type targetType;

        public CustomObjectEditorAttribute(Type targetType) { this.targetType = targetType; }
    }
}
#endif