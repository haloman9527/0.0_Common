#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  普通对象的绘制媒介
 *  Date:
 *  Version:
 *  Writer: 
 *
 */
#endregion
using CZToolKit.Core.Singletons;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;

namespace CZToolKit.Core.Editors
{
    public class ObjectInspector : CZScriptableSingleton<ObjectInspector>
    {
        [SerializeField]
        object targetObject;

        public Action onTargetObjectChanged;

        public object TargetObject { get { return targetObject; } set { targetObject = value; onTargetObjectChanged?.Invoke(); } }
    }

    [CustomEditor(typeof(ObjectInspector))]
    public class ObjectInspectorEditor : Editor
    {
        ObjectEditor objectEditor;

        private void OnEnable()
        {
            OnEnable(ObjectInspector.Instance.TargetObject);
            ObjectInspector.Instance.onTargetObjectChanged = () =>
            {
                OnEnable(ObjectInspector.Instance.TargetObject);
            };

            void OnEnable(object _targetObject)
            {
                objectEditor = ObjectEditor.CreateEditor(_targetObject);
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
            //base.OnInspectorGUI();
            if (objectEditor != null)
                objectEditor.OnInspectorGUI();
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

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            base.OnPreviewGUI(r, background);
            if (objectEditor != null)
                objectEditor.OnPreviewGUI(r, background);
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            base.OnInteractivePreviewGUI(r, background);
            if (objectEditor != null)
                objectEditor.OnInteractivePreviewGUI(r, background);
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
