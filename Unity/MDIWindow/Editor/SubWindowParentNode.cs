using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JiangeEditor
{
    [Serializable]
    public sealed class SubWindowParentNode : SubWindowNode
    {
        public const float kMaxWeight = 0.8f;
        public const float kMinWeight = 0.2f;

        private bool m_isDragging;
        private int m_currentResizeFirstId;
        private int m_currentResizeSecondId;

        public ChildrenLayout layout = ChildrenLayout.Horizontal;
        [SerializeReference] public List<SubWindowNode> children = new List<SubWindowNode>();

        public override void DoEnable(MDIWindow localEditorWindow)
        {
            this.LocalEditorWindow = localEditorWindow;
            foreach (var node in children)
            {
                node.DoEnable(localEditorWindow);
            }
        }

        public override void DoDisable()
        {
            foreach (var node in children)
            {
                node.DoDisable();
            }
        }

        public override void DoDestroy()
        {
            foreach (var node in children)
            {
                node.DoDisable();
            }
        }

        public override void DoGUI(Rect newRect)
        {
            var offset = 0f;
            switch (layout)
            {
                case ChildrenLayout.Horizontal:
                {
                    for (int i = 0; i < children.Count; i++)
                    {
                        if (i > 0)
                        {
                            this.Resize(i - 1, i, new Rect(newRect.x + offset - 2, newRect.y, 4, newRect.height));
                        }

                        var w = (int)(newRect.width * children[i].weight);
                        children[i].DoGUI(new Rect(newRect.x + offset, newRect.y, w, newRect.height));
                        if (i >= 0 && i < children.Count)
                            offset += w;
                    }

                    break;
                }
                case ChildrenLayout.Vertical:
                {
                    for (int i = 0; i < children.Count; i++)
                    {
                        if (i > 0)
                        {
                            this.Resize(i - 1, i, new Rect(position.x, position.y + offset - 2, position.width, 4));
                        }

                        var h = (int)(position.height * children[i].weight);
                        children[i].DoGUI(new Rect(position.x, position.y + offset, position.width, h));
                        if (i >= 0 && i < children.Count)
                            offset += h;
                    }

                    break;
                }
            }

            DoResize();
            this.position = newRect;
        }

        public override void DoUpdate()
        {
            foreach (var child in children)
            {
                child.DoUpdate();
            }
        }

        public override void DoInspectorUpdate()
        {
            foreach (var child in children)
            {
                child.DoInspectorUpdate();
            }
        }

        public override void DoProjectChange()
        {
            foreach (var child in children)
            {
                child.DoProjectChange();
            }
        }

        public override T OpenWindow<T>()
        {
            var window = Activator.CreateInstance<T>();
            var leaf = new SubWindowLeafNode();
            leaf.AddWindow(window);
            leaf.DoEnable(LocalEditorWindow);
            this.AddNode(leaf);
            return window;
        }

        public override SubWindow OpenWindow(Type subWindowType)
        {
            var window = (SubWindow)Activator.CreateInstance(subWindowType);
            var leaf = new SubWindowLeafNode();
            leaf.AddWindow(window);
            leaf.DoEnable(LocalEditorWindow);
            this.AddNode(leaf);
            return window;
        }

        public override void AddWindow(SubWindow window)
        {
            var leaf = new SubWindowLeafNode();
            leaf.DoEnable(LocalEditorWindow);
            leaf.AddWindow(window);
            this.AddNode(leaf);
        }

        public override void InsertWindow(SubWindow window, int index)
        {
            var leaf = new SubWindowLeafNode();
            leaf.DoEnable(LocalEditorWindow);
            leaf.AddWindow(window);
            this.InsertNode(leaf, index);
        }

        public void AddNode(SubWindowNode node)
        {
            InsertNode(node, this.children.Count);
        }

        public void InsertNode(SubWindowNode node, int index)
        {
            var w = 1.0f / (this.children.Count + 1);
            var ew = w / (this.children.Count);
            for (int i = 0; i < this.children.Count; i++)
            {
                this.children[i].weight -= ew;
            }

            node.weight = w;
            this.children.Insert(Mathf.Clamp(index, 0, children.Count), node);
        }

        public override bool DragWindow(Vector2 pos, out SubWindowParentNode parent, out SubWindowLeafNode node, out SubWindow window)
        {
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].DragWindow(pos, out parent, out node, out window))
                {
                    if (parent == null)
                    {
                        parent = this;
                    }

                    return true;
                }
            }

            parent = null;
            node = null;
            window = null;
            return false;
        }

        public override bool TriggerAnchorArea(Vector2 pos, out SubWindowParentNode parent, out SubWindowLeafNode node, out AnchorAreaDirection direction)
        {
            parent = null;
            node = null;
            direction = AnchorAreaDirection.None;
            if (!position.Contains(pos))
            {
                return false;
            }

            foreach (var child in children)
            {
                if (child.TriggerAnchorArea(pos, out parent, out node, out direction))
                {
                    if (parent == null)
                    {
                        parent = this;
                    }

                    return true;
                }
            }

            return false;
        }

        public override void Refresh()
        {
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                child.Refresh();
                if (child is SubWindowParentNode p)
                {
                    if (p.children.Count <= 0)
                    {
                        child.DoDisable();
                        child.DoDestroy();
                        children.RemoveAt(i--);
                    }
                    else if (p.children.Count == 1)
                    {
                        children[i] = p.children[0];
                        children[i].weight = p.weight;
                    }
                }
                else if (child is SubWindowLeafNode c)
                {
                    if (c.WindowCount <= 0)
                    {
                        child.DoDisable();
                        child.DoDestroy();
                        children.RemoveAt(i--);
                    }
                }
            }

            if (children.Count == 1 && children[0] is SubWindowParentNode tmpP)
            {
                this.layout = tmpP.layout;
                this.children = tmpP.children;
            }

            var tw = 0f;
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                tw += child.weight;
            }

            if (!Mathf.Approximately(tw, 1))
            {
                var o = tw - 1;
                var p = o / children.Count;
                for (int i = 0; i < children.Count; i++)
                {
                    var child = children[i];
                    child.weight -= p;
                }
            }
        }

        private void Resize(int first, int second, Rect rect)
        {
            switch (layout)
            {
                case ChildrenLayout.Horizontal:
                    EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeHorizontal);
                    break;
                case ChildrenLayout.Vertical:
                    EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeVertical);
                    break;
            }

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                if (rect.Contains(Event.current.mousePosition))
                {
                    Event.current.Use();
                    m_isDragging = true;
                    m_currentResizeFirstId = first;
                    m_currentResizeSecondId = second;
                }
            }
        }

        private void DoResize()
        {
            if (m_isDragging)
            {
                if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
                {
                    m_isDragging = false;
                    Event.current.Use();
                }

                if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
                {
                    float delta = 0;
                    switch (layout)
                    {
                        case ChildrenLayout.Horizontal:
                            delta = (Event.current.delta.x) / this.position.width;
                            break;
                        case ChildrenLayout.Vertical:
                            delta = (Event.current.delta.y) / this.position.height;
                            break;
                    }

                    float addW = children[m_currentResizeFirstId].weight + delta;
                    float musW = children[m_currentResizeSecondId].weight - delta;
                    if (delta > 0 && addW <= kMaxWeight && musW >= kMinWeight)
                    {
                        children[m_currentResizeFirstId].weight = addW;
                        children[m_currentResizeSecondId].weight = musW;
                    }
                    else if(delta <= 0 && addW >= kMinWeight && musW <= kMaxWeight)
                    {
                        children[m_currentResizeFirstId].weight = addW;
                        children[m_currentResizeSecondId].weight = musW;
                    }
                }
            }
        }
    }

    public enum ChildrenLayout
    {
        Horizontal,
        Vertical,
    }

    public enum AnchorAreaDirection
    {
        None,
        Title,
        Center,
        Top,
        Bottom,
        Left,
        Right,
    }
}