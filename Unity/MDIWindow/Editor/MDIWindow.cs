#if UNITY_EDITOR
using System;
using UnityEngine;

namespace Atom.UnityEditors
{
    [Serializable]
    public abstract class MDIWindow : BaseEditorWindow
    {
        [SerializeField] private SubWindowTree m_subWindowTree = new SubWindowTree();

        public MDIWindow Window => this;
        
        public SubWindowTree SubWindowTree => m_subWindowTree;
        
        protected virtual void OnEnable()
        {
            m_subWindowTree.Refresh();
            m_subWindowTree.DoEnable(this);
        }

        protected virtual void OnDisable()
        {
            m_subWindowTree.DoDisable();
        }

        protected virtual void OnDestroy()
        {
            m_subWindowTree.DoDestroy();
        }

        protected virtual void OnGUI()
        {
            m_subWindowTree.DoGUI(new Rect(Vector2.zero, position.size));
            this.Repaint();
        }

        protected override void Update()
        {
            base.Update();
            m_subWindowTree.DoUpdate();
        }

        private void OnInspectorUpdate()
        {
            m_subWindowTree.DoInspectorUpdate();
        }

        private void OnProjectChange()
        {
            m_subWindowTree.DoProjectChange();
        }

        public void Refresh()
        {
            m_subWindowTree.Refresh();
        }
    }
}
#endif