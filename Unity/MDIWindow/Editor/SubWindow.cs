using System;
using UnityEngine;

namespace Atom.UnityEditors
{
    [Serializable]
    public abstract class SubWindow
    {
        private bool active;
        private GUIContent m_title;
        private Rect m_position;

        public GUIContent Title
        {
            get
            {
                if (m_title == null)
                {
                    m_title = new GUIContent(this.GetType().Name);
                }

                return m_title;
            }
            set { m_title = value; }
        }

        public Rect Position => m_position;
        
        public SubWindowLeafNode LeafNode { get; private set; }

        public MDIWindow LocalEditorWindow => LeafNode.LocalEditorWindow;

        internal void SetLeafNode(SubWindowLeafNode node)
        {
            this.LeafNode = node;
        }

        public void DoEnable()
        {
            OnEnable();
        }

        public void DoDisable()
        {
            OnDisable();
        }

        public void DoDestroy()
        {
            OnDestroy();
        }

        public void DoGUI(Rect rect)
        {
            this.m_position = rect;
            OnGUI();
        }

        public void DoUpdate()
        {
            OnUpdate();
        }

        public void DoInspectorUpdate()
        {
            OnInspectorUpdate();
        }

        public void DoProjectChange()
        {
            OnProjectChange();
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }

        protected virtual void OnDestroy()
        {
        }

        protected virtual void OnGUI()
        {
        }

        protected virtual void OnUpdate()
        {
        }

        protected virtual void OnInspectorUpdate()
        {
        }

        protected virtual void OnProjectChange()
        {
        }

        public void Close()
        {
            this.LeafNode.CloseWindow(this);
        }
    }

    [Serializable]
    public class SubWindowChildren
    {
        public enum Layout
        {
            Horizontal,
            Vertical,
        }

        public Layout layout;
    }
}