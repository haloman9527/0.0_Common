#region 注 释

/***
 *
 *  Title:
 *
 *  Description:
 *  普通对象的绘制媒介
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
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace JiangeEditor
{
    public class ObjectInspector : ScriptableObject
    {
        [SerializeField]
        [SerializeReference]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.HideReferenceObjectPicker]
#endif
        public object target;

        private void OnDisable()
        {
            target = null;
        }

        public static ObjectInspector Show(object v)
        {
            var graphInspector = ScriptableObject.CreateInstance<ObjectInspector>();
            graphInspector.target = v;
            Selection.activeObject = graphInspector;
            return graphInspector;
        }
    }

    [CustomEditor(typeof(ObjectInspector))]
#if ODIN_INSPECTOR
    public class ObjectInspectorEditor : Sirenix.OdinInspector.Editor.OdinEditor
#else
    public class ObjectInspectorEditor : Editor
#endif
    {
        public static ObjectInspectorEditor CreateEditor(object target)
        {
            var inspector = ScriptableObject.CreateInstance<ObjectInspector>();
            inspector.target = target;
            return Editor.CreateEditor(inspector, typeof(ObjectInspectorEditor)) as ObjectInspectorEditor;
        }

        private ObjectEditor ObjectEditor { get; set; }

#if ODIN_INSPECTOR
        protected override void OnEnable()
        {
            base.OnEnable();
            OnEnable(target as ObjectInspector);
        }
#else
        private void OnEnable()
        {
            OnEnable(target as ObjectInspector);
        }
#endif
        
        private void OnEnable(ObjectInspector ispectorObject)
        {
            if (ispectorObject.target == null)
            {
                return;
            }
            
            ObjectEditor = ObjectEditor.CreateEditor(this);
            string title = ObjectEditor?.GetTitle();
            if (!string.IsNullOrEmpty(title))
                ispectorObject.name = title;
            ObjectEditor?.OnEnable();
        }

#if ODIN_INSPECTOR
        protected override void OnDisable()
        {
            ObjectEditor?.OnDisable();
            ObjectEditor = null;
            base.OnDisable();
        }
#else
        private void OnDisable()
        {
            ObjectEditor?.OnDisable();
            ObjectEditor = null;
        }
#endif

        protected override void OnHeaderGUI() => ObjectEditor?.OnHeaderGUI();
        public void BaseOnHeaderGUI() => base.OnHeaderGUI();

        public override VisualElement CreateInspectorGUI() => ObjectEditor?.CreateInspectorGUI();
        public VisualElement BaseCreateInspectorGUI() => base.CreateInspectorGUI();

        public override void OnInspectorGUI() => ObjectEditor?.OnInspectorGUI();
        public void BaseOnInspecotrGUI() => base.OnInspectorGUI();

        public override void DrawPreview(Rect previewArea) => ObjectEditor?.DrawPreview(previewArea);
        public void BaseDrawPreview(Rect previewArea) => base.DrawPreview(previewArea);

        public override bool HasPreviewGUI() => ObjectEditor == null ? BaseHasPreviewGUI() : ObjectEditor.HasPreviewGUI();
        public bool BaseHasPreviewGUI() => base.HasPreviewGUI();

        public override void OnPreviewSettings() => ObjectEditor?.OnPreviewSettings();
        public void BasePreviewSettings() => base.OnPreviewSettings();

        public override GUIContent GetPreviewTitle() => ObjectEditor?.GetPreviewTitle();
        public GUIContent BaseGetPreviewTitle() => base.GetPreviewTitle();

        public override void OnPreviewGUI(Rect rect, GUIStyle background) => ObjectEditor?.OnPreviewGUI(rect, background);
        public void BaseOnPreviewGUI(Rect rect, GUIStyle background) => base.OnPreviewGUI(rect, background);

        public override void OnInteractivePreviewGUI(Rect rect, GUIStyle background) => ObjectEditor?.OnInteractivePreviewGUI(rect, background);
        public void BaseOnInteractivePreviewGUI(Rect rect, GUIStyle background) => base.OnInteractivePreviewGUI(rect, background);

        private void OnSceneGUI()
        {
            ObjectEditor?.OnSceneGUI();
        }
        
        private void OnValidate()
        {
            ObjectEditor?.OnValidate();
        }
    }
}
#endif