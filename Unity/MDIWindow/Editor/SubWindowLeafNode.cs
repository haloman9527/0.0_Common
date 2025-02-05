using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Moyo.UnityEditors
{
    [Serializable]
    public sealed class SubWindowLeafNode : SubWindowNode
    {
        public int selectedWindowIndex = 0;
        [SerializeReference] public List<SubWindow> windows = new List<SubWindow>();

        public int WindowCount => windows.Count;

        public override void DoEnable(MDIWindow localEditorWindow)
        {
            this.LocalEditorWindow = localEditorWindow;
            for (int i = 0; i < windows.Count; i++)
            {
                windows[i].SetLeafNode(this);
                windows[i].DoEnable();
            }
        }

        public override void DoDisable()
        {
            for (int i = 0; i < windows.Count; i++)
            {
                windows[i].DoDisable();
            }
        }

        public override void DoDestroy()
        {
            for (int i = 0; i < windows.Count; i++)
            {
                windows[i].DoDestroy();
            }
        }

        public override void DoGUI(Rect newRect)
        {
            this.position = newRect;
            if (selectedWindowIndex >= windows.Count)
            {
                selectedWindowIndex = windows.Count - 1;
            }

            GUI.Box(GetArea(AnchorAreaDirection.Title), string.Empty, "dockarea");
            var lastWidth = 0f;
            for (int i = 0; i < windows.Count; i++)
            {
                var tabTitleArea = TabTitleArea(i, lastWidth);
                lastWidth += tabTitleArea.width;
                if (Event.current.type == EventType.MouseDown && Event.current.button == 2 && tabTitleArea.Contains(Event.current.mousePosition))
                {
                    EditorApplication.delayCall += () => { windows[selectedWindowIndex].Close(); };
                    Event.current.Use();
                }

                if (selectedWindowIndex == i)
                {
                    GUI.Box(tabTitleArea, GUIContent.none, "FrameBox");
                    GUI.Label(tabTitleArea, windows[selectedWindowIndex].Title, "dragtab");
                }
                else if (GUI.Button(tabTitleArea, windows[i].Title, "dragtab"))
                {
                    selectedWindowIndex = i;
                }
            }

            var contentArea = this.ContentArea();
            GUI.Box(contentArea, string.Empty, "FrameBox");
            if (selectedWindowIndex >= 0 && windows.Count > selectedWindowIndex)
            {
                var indentContentArea = contentArea;
                indentContentArea.xMin += 2;
                indentContentArea.xMax -= 2;
                indentContentArea.yMin += 2;
                indentContentArea.yMax -= 2;
                using (new GUILayout.AreaScope(indentContentArea))
                {
                    windows[selectedWindowIndex].DoGUI(indentContentArea);
                }
            }
        }

        public override void DoUpdate()
        {
            for (int i = 0; i < windows.Count; i++)
            {
                windows[i].DoUpdate();
            }
        }

        public override void DoInspectorUpdate()
        {
            for (int i = 0; i < windows.Count; i++)
            {
                windows[i].DoInspectorUpdate();
            }
        }

        public override void DoProjectChange()
        {
            for (int i = 0; i < windows.Count; i++)
            {
                windows[i].DoProjectChange();
            }
        }

        public override T OpenWindow<T>()
        {
            var subWindow = Activator.CreateInstance<T>();
            this.AddWindow(subWindow);
            subWindow.SetLeafNode(this);
            subWindow.DoEnable();
            return subWindow;
        }

        public override SubWindow OpenWindow(Type subWindowType)
        {
            var subWindow = (SubWindow)Activator.CreateInstance(subWindowType);
            this.AddWindow(subWindow);
            subWindow.SetLeafNode(this);
            subWindow.DoEnable();
            return subWindow;
        }

        public void CloseWindow(SubWindow window)
        {
            if (!this.windows.Contains(window))
            {
                return;
            }

            this.windows.Remove(window);
            window.DoDisable();
            window.DoDestroy();
            LocalEditorWindow.Refresh();
        }

        public override void AddWindow(SubWindow window)
        {
            this.windows.Insert(windows.Count, window);
            window.SetLeafNode(this);
            this.selectedWindowIndex = windows.Count - 1;
        }

        public override void InsertWindow(SubWindow window, int index)
        {
            this.windows.Insert(index, window);
            window.SetLeafNode(this);
            this.selectedWindowIndex = index;
        }

        public void RemoveWindow(SubWindow window)
        {
            this.windows.Remove(window);
        }

        public override bool DragWindow(Vector2 pos, out SubWindowParentNode parent, out SubWindowLeafNode node, out SubWindow window)
        {
            var lastWidth = 0f;
            for (int i = 0; i < windows.Count; i++)
            {
                var titleRect = TabTitleArea(i, lastWidth);
                lastWidth += titleRect.width;
                if (titleRect.Contains(pos))
                {
                    parent = null;
                    node = this;
                    window = windows[i];
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

            var titleArea = GetArea(AnchorAreaDirection.Title);
            if (titleArea.Contains(pos))
            {
                node = this;
                direction = AnchorAreaDirection.Title;
                return true;
            }

            var leftRect = GetArea(AnchorAreaDirection.Left);
            if (leftRect.Contains(pos))
            {
                node = this;
                direction = AnchorAreaDirection.Left;
                return true;
            }

            var rightRect = GetArea(AnchorAreaDirection.Right);
            if (rightRect.Contains(pos))
            {
                node = this;
                direction = AnchorAreaDirection.Right;
                return true;
            }

            var topRect = GetArea(AnchorAreaDirection.Top);
            if (topRect.Contains(pos))
            {
                node = this;
                direction = AnchorAreaDirection.Top;
                return true;
            }

            var bottomRect = GetArea(AnchorAreaDirection.Bottom);
            if (bottomRect.Contains(pos))
            {
                node = this;
                direction = AnchorAreaDirection.Bottom;
                return true;
            }

            node = this;
            direction = AnchorAreaDirection.Center;
            return true;
        }

        public override void Refresh()
        {
            for (int i = 0; i < windows.Count; i++)
            {
                var window = windows[i];
                if (window == null)
                {
                    windows.RemoveAt(i--);
                }
            }
        }

        public Rect GetArea(AnchorAreaDirection direction)
        {
            var anchorAreaW = position.width * 0.2f;
            var anchorAreaH = position.height * 0.2f;
            switch (direction)
            {
                case AnchorAreaDirection.Title:
                    return new Rect(position.x, position.y, position.width, 18);
                case AnchorAreaDirection.Center:
                    return new Rect(position.x + anchorAreaW, position.y + anchorAreaH, position.width - anchorAreaW - anchorAreaW, position.height - anchorAreaH - anchorAreaH);
                case AnchorAreaDirection.Top:
                    return new Rect(position.x, position.y, position.width, anchorAreaH);
                case AnchorAreaDirection.Bottom:
                    return new Rect(position.x, position.yMax - anchorAreaH, position.width, anchorAreaH);
                case AnchorAreaDirection.Left:
                    return new Rect(position.x, position.y, anchorAreaW, position.height);
                case AnchorAreaDirection.Right:
                    return new Rect(position.xMax - anchorAreaW, position.y, anchorAreaW, position.height);
            }

            throw new Exception();
        }

        private Rect TabTitleArea(int index, float lastWidth)
        {
            var tabWidth = ((GUIStyle)"dragtab").CalcSize(windows[index].Title).x;
            return new Rect(position.x + lastWidth, position.y, tabWidth, 18);
        }

        private Rect ContentArea()
        {
            return new Rect(position.x, position.y + 18, position.width, position.height - 18);
        }
    }
}