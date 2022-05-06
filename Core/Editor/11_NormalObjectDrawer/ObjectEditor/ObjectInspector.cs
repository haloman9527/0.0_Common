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
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */
#endregion
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;

using UnityObject = UnityEngine.Object;

namespace CZToolKit.Core.Editors
{
    public class ObjectInspector : Singletons.ScriptableSingleton<ObjectInspector>
    {
        [SerializeField]
        object target;

        public Action onTargetChanged;

        public object Target
        {
            get { return target; }
            private set
            {
                if (target != value)
                {
                    target = value;
                    onTargetChanged?.Invoke();
                }
            }
        }
        public UnityObject UnityContext { get; private set; }

        public void Initialize(object target, UnityObject unityContext)
        {
            UnityContext = unityContext;
            Target = target;
        }
    }

    [CustomEditor(typeof(ObjectInspector))]
#if ODIN_INSPECTOR && DRAW_WITH_ODIN
    public class ObjectInspectorEditor : Sirenix.OdinInspector.Editor.OdinEditor
#else
    public class ObjectInspectorEditor : Editor
#endif
    {
        public ObjectEditor ObjectEditor
        {
            get;
            set;
        }
        public ObjectInspector InspectorObject
        {
            get { return target as ObjectInspector; }
        }
#if ODIN_INSPECTOR && DRAW_WITH_ODIN
        protected override void OnEnable()
        {
            base.OnEnable();
#else
        void OnEnable()
        {
#endif
            OnEnable(InspectorObject);
            void OnEnable(ObjectInspector ispectorObject)
            {
                if (ispectorObject == null)
                    return;
                if (ispectorObject.Target == null)
                    return;
                ispectorObject.onTargetChanged = () =>
                {
                    OnEnable(ispectorObject);
                };
                ObjectEditor = ObjectEditor.CreateEditor(ispectorObject.Target, ispectorObject.UnityContext, this);
                if (ObjectEditor == null)
                    return;
                string title = ObjectEditor.GetTitle();
                if (!string.IsNullOrEmpty(title))
                    ispectorObject.name = title;
                ObjectEditor.OnEnable();
            }
        }

        protected override void OnHeaderGUI()
        {
            base.OnHeaderGUI();
            if (ObjectEditor != null)
                ObjectEditor.OnHeaderGUI();
        }

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement v = null;
            if (ObjectEditor != null)
                v = ObjectEditor.CreateInspectorGUI();
            if (v == null)
                v = base.CreateInspectorGUI();
            return v;
        }

        public override void OnInspectorGUI()
        {
            if (ObjectEditor.Target == null)
                ObjectEditor = null;
            if (ObjectEditor != null)
                ObjectEditor.OnInspectorGUI();
        }

        public void DrawBaseInspecotrGUI()
        {
            base.OnInspectorGUI();
        }

        public override void DrawPreview(Rect previewArea)
        {
            base.DrawPreview(previewArea);
            if (ObjectEditor != null)
                ObjectEditor.DrawPreview(previewArea);
        }

        public override bool HasPreviewGUI()
        {
            if (ObjectEditor != null)
                return ObjectEditor.HasPreviewGUI();
            return base.HasPreviewGUI();
        }

        public override void OnPreviewSettings()
        {
            base.OnPreviewSettings();
            if (ObjectEditor != null)
                ObjectEditor.OnPreviewSettings();
        }

        public override GUIContent GetPreviewTitle()
        {
            GUIContent title = null;
            if (ObjectEditor != null)
                title = ObjectEditor.GetPreviewTitle();
            if (title == null)
                title = base.GetPreviewTitle();
            return title;
        }

        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            base.OnPreviewGUI(rect, background);
            if (ObjectEditor != null)
                ObjectEditor.OnPreviewGUI(rect, background);
        }

        public override void OnInteractivePreviewGUI(Rect rect, GUIStyle background)
        {
            base.OnInteractivePreviewGUI(rect, background);
            if (ObjectEditor != null)
                ObjectEditor.OnInteractivePreviewGUI(rect, background);
        }

        private void OnSceneGUI()
        {
            if (ObjectEditor != null)
                ObjectEditor.OnSceneGUI();
        }

        private void OnValidate()
        {
            if (ObjectEditor != null)
                ObjectEditor.OnValidate();
        }

        private void OnDisable()
        {
            if (ObjectEditor != null)
                ObjectEditor.OnDisable();
            ObjectEditor = null;
        }
    }
}
#endif