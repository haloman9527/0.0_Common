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
using UnityEngine.UIElements;
using Atom;

namespace Atom.UnityEditors
{
    public class ObjectEditor
    {
        public ObjectInspectorEditor SourceEditor { get; private set; }

        public ObjectInspector Source => (ObjectInspector)SourceEditor.target;

        public object Target => Source.target;
        
        public MonoScript Script { get; private set; }

        void Initialize(ObjectInspectorEditor sourceEditor)
        {
            SourceEditor = sourceEditor;
            Script = EditorUtilityExtension.FindScriptFromType(Target.GetType());
        }

        public virtual string GetTitle()
        {
            return string.Empty;
        }

        public virtual void OnEnable()
        {
        }

        public virtual void OnDisable()
        {
        }

        public virtual void OnValidate()
        {
        }

        public virtual void OnSceneGUI()
        {
        }

        public virtual VisualElement CreateInspectorGUI()
        {
            return SourceEditor.BaseCreateInspectorGUI();
        }

        public virtual void OnHeaderGUI()
        {
            SourceEditor.BaseOnHeaderGUI();
        }

        public virtual void OnInspectorGUI()
        {
            SourceEditor.BaseOnInspecotrGUI();
        }

        public virtual bool HasPreviewGUI()
        {
            return SourceEditor.BaseHasPreviewGUI();
        }

        public virtual GUIContent GetPreviewTitle()
        {
            return SourceEditor.BaseGetPreviewTitle();
        }

        public virtual void OnPreviewSettings()
        {
            SourceEditor.BasePreviewSettings();
        }

        public virtual void DrawPreview(Rect previewArea)
        {
            SourceEditor.BaseDrawPreview(previewArea);
        }

        public virtual void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            SourceEditor.BaseOnPreviewGUI(rect, background);
        }

        public virtual void OnInteractivePreviewGUI(Rect rect, GUIStyle background)
        {
            SourceEditor.BaseOnInteractivePreviewGUI(rect, background);
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
                foreach (var attr in ReflectionEx.GetTypeAttributes<CustomObjectEditorAttribute>(type, true))
                {
                    ObjectEditorTypeCache[attr.type] = type;
                }
            }
        }

        private static Type GetEditorType(Type objectType)
        {
            if (ObjectEditorTypeCache.TryGetValue(objectType, out Type editorType))
                return editorType;
            if (objectType.BaseType != null)
                return GetEditorType(objectType.BaseType);
            return null;
        }

        public static bool HasEditor(Type targetType)
        {
            if (ObjectEditorTypeCache.TryGetValue(targetType, out Type editorType))
                return true;
            if (targetType.BaseType != null)
                return HasEditor(targetType.BaseType);
            return false;
        }

        public static ObjectEditor CreateEditor(ObjectInspectorEditor sourceEditor)
        {
            var objectInspector = (ObjectInspector)sourceEditor.target;
            var editorType = GetEditorType(objectInspector.target.GetType());
            if (editorType == null)
                editorType = typeof(ObjectEditor);
            var objectEditor = Activator.CreateInstance(editorType, true) as ObjectEditor;
            if (objectEditor == null)
                return null;
            objectEditor.Initialize(sourceEditor);
            return objectEditor;
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