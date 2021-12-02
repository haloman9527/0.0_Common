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
using CZToolKit.Core.Singletons;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;

using UnityObject = UnityEngine.Object;

namespace CZToolKit.Core.Editors
{
    public class ObjectInspector : CZScriptableSingleton<ObjectInspector>
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
        ObjectEditor objectEditor;

        public UnityObject UnityContext { get; set; }

        public ObjectInspector T_Target { get { return target as ObjectInspector; } }
#if ODIN_INSPECTOR && DRAW_WITH_ODIN
        protected override void OnEnable()
        {
            base.OnEnable();
#else
        void OnEnable()
        {
#endif
            UnityContext = T_Target.UnityContext;
            OnEnable(T_Target.Target);
            T_Target.onTargetChanged = () =>
            {
                OnEnable(T_Target.Target);
            };
            void OnEnable(object _targetObject)
            {
                objectEditor = ObjectEditor.CreateEditor(_targetObject, UnityContext, this);
                if (objectEditor != null)
                {
                    string title = objectEditor.GetTitle();
                    if (!string.IsNullOrEmpty(title))
                        target.name = title;
                    objectEditor.OnEnable();
                }
            }
        }

        protected override void OnHeaderGUI()
        {
            base.OnHeaderGUI();
            if (objectEditor != null)
                objectEditor.OnHeaderGUI();
        }

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement v = null;
            if (objectEditor != null)
                v = objectEditor.CreateInspectorGUI();
            if (v == null)
                v = base.CreateInspectorGUI();
            return v;
        }

        public override void OnInspectorGUI()
        {
            if (objectEditor != null)
                objectEditor.OnInspectorGUI();
        }

        public void DrawBaseInspecotrGUI()
        {
            base.OnInspectorGUI();
        }

        public override void DrawPreview(Rect previewArea)
        {
            base.DrawPreview(previewArea);
            if (objectEditor != null)
                objectEditor.DrawPreview(previewArea);
        }

        public override bool HasPreviewGUI()
        {
            if (objectEditor != null)
                return objectEditor.HasPreviewGUI();
            return base.HasPreviewGUI();
        }

        public override void OnPreviewSettings()
        {
            base.OnPreviewSettings();
            if (objectEditor != null)
                objectEditor.OnPreviewSettings();
        }

        public override GUIContent GetPreviewTitle()
        {
            GUIContent title = null;
            if (objectEditor != null)
                title = objectEditor.GetPreviewTitle();
            if (title == null)
                title = base.GetPreviewTitle();
            return title;
        }

        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            base.OnPreviewGUI(rect, background);
            if (objectEditor != null)
                objectEditor.OnPreviewGUI(rect, background);
        }

        public override void OnInteractivePreviewGUI(Rect rect, GUIStyle background)
        {
            base.OnInteractivePreviewGUI(rect, background);
            if (objectEditor != null)
                objectEditor.OnInteractivePreviewGUI(rect, background);
        }

        private void OnSceneGUI()
        {
            if (objectEditor != null)
                objectEditor.OnSceneGUI();
        }

        private void OnValidate()
        {
            if (objectEditor != null)
                objectEditor.OnValidate();
        }

        private void OnDisable()
        {
            if (objectEditor != null)
                objectEditor.OnDisable();
        }
    }
}
#endif