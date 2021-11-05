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

        public static ObjectEditor CreateEditor(object _targetObject)
        {
            ObjectEditor objectEditor = InternalCreateEditor(_targetObject);
            if (objectEditor == null) return null;

            objectEditor.Initialize(_targetObject);
            return objectEditor;
        }

        public static ObjectEditor CreateEditor(object _targetObject, UnityObject _owner)
        {
            ObjectEditor objectEditor = InternalCreateEditor(_targetObject);
            if (objectEditor == null) return null;

            objectEditor.Initialize(_targetObject, _owner);
            return objectEditor;
        }

        public static ObjectEditor CreateEditor(object _targetObject, UnityObject _owner, Editor _editor)
        {
            ObjectEditor objectEditor = InternalCreateEditor(_targetObject);
            if (objectEditor == null) return null;

            objectEditor.Initialize(_targetObject, _owner, _editor);
            return objectEditor;
        }

        static ObjectEditor InternalCreateEditor(object _targetObject)
        {
            if (_targetObject == null) return null;
            return Activator.CreateInstance(GetEditorType(_targetObject.GetType()), true) as ObjectEditor;
        }

        protected IReadOnlyList<FieldInfo> Fields { get; private set; }

        public object Target { get; private set; }
        public UnityObject Owner { get; private set; }
        public Editor Editor { get; private set; }
        public MonoScript Script { get; private set; }

        protected ObjectEditor() { }

        void Initialize(object _target)
        {
            Target = _target;
            Script = EditorUtilityExtension.FindScriptFromType(Target.GetType());
            Fields = Util_Reflection.GetFieldInfos(Target.GetType()).Where(field => EditorGUILayoutExtension.CanDraw(field)).ToList();
        }

        void Initialize(object _target, UnityObject _owner)
        {
            Owner = _owner;
            Initialize(_target);
        }

        void Initialize(object _target, UnityObject _owner, Editor _editor)
        {
            Owner = _owner;
            Editor = _editor;
            Initialize(_target);
        }

        public virtual string GetTitle() { return string.Empty; }

        public virtual void OnEnable() { }

        public virtual void OnHeaderGUI() { }

        public virtual void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", Script, typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();
            foreach (var field in Fields)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayoutExtension.DrawField(field, Target);
                if (EditorGUI.EndChangeCheck())
                    GUI.changed = true;
            }
        }

        public virtual bool HasPreviewGUI() { return false; }

        public virtual GUIContent GetPreviewTitle() { return null; }

        public virtual void OnPreviewSettings() { }

        public virtual void DrawPreview(Rect previewArea) { }

        public virtual void OnPreviewGUI(Rect _r, GUIStyle _background) { }

        public virtual void OnInteractivePreviewGUI(Rect _r, GUIStyle _background) { }

        public virtual void OnValidate() { }

        public virtual void OnSceneGUI() { }

        public virtual void OnDisable() { }

        public VisualElement CreateInspectorGUI() { return null; }
    }

    public class CustomObjectEditorAttribute : Attribute
    {
        public Type targetType;

        public CustomObjectEditorAttribute(Type _targetType) { targetType = _targetType; }
    }
}
#endif