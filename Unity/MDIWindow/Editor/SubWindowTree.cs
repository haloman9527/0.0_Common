using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atom.UnityEditors
{
    [Serializable]
    public sealed class SubWindowTree
    {
        [SerializeField] public SubWindowParentNode m_root = new SubWindowParentNode();
        private DraggingWindowData m_currentDrag;

        public MDIWindow LocalEditorWindow { get; private set; }

        public void Refresh()
        {
            if (m_root == null)
            {
                m_root = new SubWindowParentNode();
            }

            m_root.Refresh();
        }

        public void DoEnable(MDIWindow localEditorWindow)
        {
            this.LocalEditorWindow = localEditorWindow;
            if (m_root == null)
            {
                m_root = new SubWindowParentNode();
            }

            m_root.DoEnable(localEditorWindow);
        }

        public void DoDisable()
        {
            m_root.DoDisable();
        }

        public void DoDestroy()
        {
            m_root.DoDestroy();
        }

        public void DoGUI(Rect position)
        {
            m_root.DoGUI(position);
            this.DragWindow();
        }

        public void DoUpdate()
        {
            m_root.DoUpdate();
        }

        public void DoInspectorUpdate()
        {
            m_root.DoInspectorUpdate();
        }

        public void DoProjectChange()
        {
            m_root.DoProjectChange();
        }

        public T FindWindow<T>() where T : SubWindow
        {
            foreach (var node in Nodes(m_root))
            {
                if (!(node is SubWindowLeafNode leafNode))
                {
                    continue;
                }

                foreach (var window in leafNode.windows)
                {
                    if (!(window is T w))
                    {
                        continue;
                    }

                    return w;
                }
            }

            return null;
        }

        private IEnumerable<SubWindowNode> Nodes(SubWindowNode root)
        {
            yield return root;
            if (root is SubWindowParentNode parentNode)
            {
                foreach (var node in parentNode.children)
                {
                    foreach (var child in Nodes(node))
                    {
                        yield return child;
                    }
                }
            }
        }

        public T AddDynamicWindow<T>() where T : SubWindow
        {
            return this.m_root.OpenWindow<T>();
        }

        public SubWindow AddDynamicWindow(Type subWindowType)
        {
            return this.m_root.OpenWindow(subWindowType);
        }

        private void DragWindow()
        {
            var position = Event.current.mousePosition;
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                if (m_root.DragWindow(position, out var parent, out var node, out var window))
                {
                    this.m_currentDrag = new DraggingWindowData()
                    {
                        parentNode = parent,
                        leafNode = node,
                        window = window,
                    };
                }
            }

            if (m_currentDrag != null && Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                try
                {
                    if (m_root.TriggerAnchorArea(position, out var tmpParent, out var tmpNode, out var tmpDirection)
                        && (m_currentDrag.leafNode != tmpNode || m_currentDrag.leafNode.WindowCount != 1))
                    {
                        switch (tmpDirection)
                        {
                            case AnchorAreaDirection.Title:
                            {
                                this.m_currentDrag.leafNode.RemoveWindow(m_currentDrag.window);
                                tmpNode.AddWindow(m_currentDrag.window);
                                LocalEditorWindow.Refresh();
                                break;
                            }
                            case AnchorAreaDirection.Top:
                            {
                                this.m_currentDrag.leafNode.RemoveWindow(m_currentDrag.window);
                                if (this.m_currentDrag.leafNode.WindowCount == 0)
                                {
                                    this.m_currentDrag.parentNode.children.Remove(this.m_currentDrag.leafNode);
                                }

                                if (this.m_currentDrag.parentNode.children.Count == 1)
                                {
                                    this.m_currentDrag.parentNode.children[0].weight = 1;
                                }

                                if (tmpParent.layout == ChildrenLayout.Vertical)
                                {
                                    tmpParent.InsertWindow(m_currentDrag.window, tmpParent.children.IndexOf(tmpNode));
                                }
                                else
                                {
                                    if (tmpParent.children.Count == 1)
                                    {
                                        tmpParent.layout = ChildrenLayout.Vertical;
                                        tmpParent.InsertWindow(m_currentDrag.window, 0);
                                    }
                                    else
                                    {
                                        var index = tmpParent.children.IndexOf(tmpNode);
                                        var newParent = new SubWindowParentNode() { layout = ChildrenLayout.Vertical, weight = tmpNode.weight };
                                        newParent.DoEnable(LocalEditorWindow);
                                        newParent.AddNode(tmpNode);
                                        tmpParent.children[index] = newParent;
                                        newParent.InsertWindow(m_currentDrag.window, 0);
                                    }
                                }

                                LocalEditorWindow.Refresh();
                                break;
                            }
                            case AnchorAreaDirection.Bottom:
                            {
                                this.m_currentDrag.leafNode.RemoveWindow(m_currentDrag.window);
                                if (this.m_currentDrag.leafNode.WindowCount == 0)
                                {
                                    this.m_currentDrag.parentNode.children.Remove(this.m_currentDrag.leafNode);
                                }

                                if (this.m_currentDrag.parentNode.children.Count == 1)
                                {
                                    this.m_currentDrag.parentNode.children[0].weight = 1;
                                }

                                if (tmpParent.layout == ChildrenLayout.Vertical)
                                {
                                    tmpParent.InsertWindow(m_currentDrag.window, tmpParent.children.IndexOf(tmpNode) + 1);
                                }
                                else
                                {
                                    if (tmpParent.children.Count == 1)
                                    {
                                        tmpParent.layout = ChildrenLayout.Vertical;
                                        tmpParent.AddWindow(m_currentDrag.window);
                                    }
                                    else
                                    {
                                        var index = tmpParent.children.IndexOf(tmpNode);
                                        var newParent = new SubWindowParentNode() { layout = ChildrenLayout.Vertical, weight = tmpNode.weight };
                                        newParent.DoEnable(LocalEditorWindow);
                                        newParent.AddNode(tmpNode);
                                        tmpParent.children[index] = newParent;
                                        newParent.AddWindow(m_currentDrag.window);
                                    }
                                }

                                LocalEditorWindow.Refresh();
                                break;
                            }
                            case AnchorAreaDirection.Left:
                            {
                                this.m_currentDrag.leafNode.RemoveWindow(m_currentDrag.window);
                                if (this.m_currentDrag.leafNode.WindowCount == 0)
                                {
                                    this.m_currentDrag.parentNode.children.Remove(this.m_currentDrag.leafNode);
                                }

                                if (this.m_currentDrag.parentNode.children.Count == 1)
                                {
                                    this.m_currentDrag.parentNode.children[0].weight = 1;
                                }

                                if (tmpParent.layout == ChildrenLayout.Horizontal)
                                {
                                    tmpParent.InsertWindow(m_currentDrag.window, tmpParent.children.IndexOf(tmpNode));
                                }
                                else
                                {
                                    if (tmpParent.children.Count == 1)
                                    {
                                        tmpParent.layout = ChildrenLayout.Horizontal;
                                        tmpParent.InsertWindow(m_currentDrag.window, 0);
                                    }
                                    else
                                    {
                                        var index = tmpParent.children.IndexOf(tmpNode);
                                        var newParent = new SubWindowParentNode() { layout = ChildrenLayout.Horizontal, weight = tmpNode.weight };
                                        newParent.DoEnable(LocalEditorWindow);
                                        newParent.AddNode(tmpNode);
                                        tmpParent.children[index] = newParent;
                                        newParent.InsertWindow(m_currentDrag.window, 0);
                                    }
                                }

                                LocalEditorWindow.Refresh();
                                break;
                            }
                            case AnchorAreaDirection.Right:
                            {
                                this.m_currentDrag.leafNode.RemoveWindow(m_currentDrag.window);
                                if (this.m_currentDrag.leafNode.WindowCount == 0)
                                {
                                    this.m_currentDrag.parentNode.children.Remove(this.m_currentDrag.leafNode);
                                }

                                if (this.m_currentDrag.parentNode.children.Count == 1)
                                {
                                    this.m_currentDrag.parentNode.children[0].weight = 1;
                                }

                                if (tmpParent.layout == ChildrenLayout.Horizontal)
                                {
                                    tmpParent.InsertWindow(m_currentDrag.window, tmpParent.children.IndexOf(tmpNode) + 1);
                                }
                                else
                                {
                                    if (tmpParent.children.Count == 1)
                                    {
                                        tmpParent.layout = ChildrenLayout.Horizontal;
                                        tmpParent.AddWindow(m_currentDrag.window);
                                    }
                                    else
                                    {
                                        var index = tmpParent.children.IndexOf(tmpNode);
                                        var newParent = new SubWindowParentNode() { layout = ChildrenLayout.Horizontal, weight = tmpNode.weight };
                                        newParent.DoEnable(LocalEditorWindow);
                                        newParent.AddNode(tmpNode);
                                        tmpParent.children[index] = newParent;
                                        newParent.AddWindow(m_currentDrag.window);
                                    }
                                }

                                LocalEditorWindow.Refresh();
                                break;
                            }
                        }
                    }
                }
                finally
                {
                    this.m_currentDrag = null;
                }
            }

            if (m_currentDrag != null)
            {
                if (m_root.TriggerAnchorArea(position, out var tmpParent, out var tmpNode, out var tmpDirection))
                {
                    switch (tmpDirection)
                    {
                        case AnchorAreaDirection.Title:
                        case AnchorAreaDirection.Top:
                        case AnchorAreaDirection.Bottom:
                        case AnchorAreaDirection.Left:
                        case AnchorAreaDirection.Right:
                        {
                            GUI.Box(tmpNode.GetArea(tmpDirection), GUIContent.none, "SelectionRect");
                            break;
                        }
                    }
                }

                GUI.Box(new Rect(position.x - 50, position.y - 10, 100, 40), m_currentDrag.window.Title, "window");
            }
        }
    }

    public class DraggingWindowData
    {
        public SubWindowParentNode parentNode;
        public SubWindowLeafNode leafNode;
        public SubWindow window;
    }
}