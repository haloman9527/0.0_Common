#if UNITY_EDITOR
using System;
using UnityEngine;

namespace Atom.UnityEditors
{
    [Serializable]
    public abstract class SubWindowNode
    {
        /// <summary>
        /// 权重-（所有兄弟节点的权重和应为1）
        /// </summary>
        public float weight = 1;

        protected Rect position;
        
        public MDIWindow LocalEditorWindow { get; protected set; }

        public abstract void DoEnable(MDIWindow localEditorWindow);

        public abstract void DoDisable();

        public abstract void DoDestroy();

        public abstract void DoGUI(Rect rect);

        public abstract void DoUpdate();

        public abstract void DoInspectorUpdate();
        
        public abstract void DoProjectChange();

        public abstract T OpenWindow<T>() where T : SubWindow;

        public abstract SubWindow OpenWindow(Type windowType);

        public abstract void AddWindow(SubWindow window);
        
        public abstract void InsertWindow(SubWindow window, int index);

        public abstract void Refresh();

        public abstract bool DragWindow(Vector2 pos, out SubWindowParentNode parent, out SubWindowLeafNode node, out SubWindow window);

        public abstract bool TriggerAnchorArea(Vector2 pos, out SubWindowParentNode parent, out SubWindowLeafNode node, out AnchorAreaDirection direction);

        
    }
}
#endif