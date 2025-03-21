#if UNITY_EDITOR
using System;
using Atom.UnityEditors.IMGUI.Controls;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;
using TreeView = Atom.UnityEditors.IMGUI.Controls.TreeView;

namespace Atom.UnityEditors
{
    using TreeView = IMGUI.Controls.TreeView;

    public abstract class MenuEditorWindow : EditorWindow
    {
        public bool showMenu = true;
        [SerializeField] private ResizableArea resizableArea;
        private VisualElement rightContentVisualElement;
        private Vector2 leftScroll;
        private Rect rightRect;
        private Rect sideRect;
        private Rect contentRect;
        private SearchField menuTreeViewSearchField;

        public ResizableArea ResizableArea
        {
            get
            {
                if (resizableArea == null)
                {
                    resizableArea = new ResizableArea
                    {
                        rect = new Rect(0, 0, 150, 150),
                        minSize = this.minSize,
                        side = 10
                    };
                    resizableArea.EnableSide(ResizableArea.UIDirection.Right);
                }

                return resizableArea;
            }
        }

        public VisualElement RightContentVisualElement
        {
            get
            {
                if (rightContentVisualElement == null)
                {
                    rightContentVisualElement = new VisualElement();
                    rightContentVisualElement.pickingMode = PickingMode.Ignore;
                    rootVisualElement.Add(rightContentVisualElement);
                }

                return rightContentVisualElement;
            }
        }

        private SearchField MenuTreeViewSearchField
        {
            get
            {
                if (menuTreeViewSearchField == null)
                {
                    menuTreeViewSearchField = new SearchField();
                }

                return menuTreeViewSearchField;
            }
        }

        public abstract TreeView MenuTreeView { get; }

        private void ShowButton(Rect rect)
        {
#if UNITY_6000_0_OR_NEWER
            rect.x = 0;
            rect.y = 5;
            rect.height = 20;
            rect.width = position.width - 25;
#else
            rect.x = 0;
            rect.y = 1;
            rect.height = 20;
            rect.width = position.width - 60;
#endif
            GUILayout.BeginArea(rect);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            OnShowButton(rect);

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        protected virtual void OnShowButton(Rect rect)
        {
            showMenu = GUILayout.Toggle(showMenu, "Menu", GUI.skin.button);
        }

        protected virtual void OnGUI()
        {
            if (showMenu)
            {
                var r = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandHeight(true), GUILayout.ExpandHeight(true));
                if (Event.current.type == EventType.Repaint)
                {
                    ResizableArea.rect.position = r.position;
                    ResizableArea.maxSize = r.size;
                    ResizableArea.rect.height = r.height;
                }

                ResizableArea.OnGUI();

                sideRect = ResizableArea.rect;
                sideRect.x += sideRect.width;
                sideRect.width = 1;

                rightRect = sideRect;
                rightRect.x += rightRect.width + 1;
                rightRect.width = position.width - ResizableArea.rect.width - sideRect.width - 2;
                rightRect.width = Mathf.Max(rightRect.width, 100);
                rightRect.xMin += 10;
                rightRect.xMax -= 10;

                GUILayout.BeginArea(ResizableArea.rect);
                OnLeftGUI(new Rect(Vector2.zero, ResizableArea.rect.size));
                GUILayout.EndArea();

                EditorGUI.DrawRect(sideRect, new Color(0.5f, 0.5f, 0.5f, 1));
            }
            else
            {
                rightRect = new Rect(Vector2.zero, position.size);
                rightRect.xMin += 11;
                rightRect.xMax -= 11;
            }

            RightContentVisualElement.style.position = Position.Relative;
            RightContentVisualElement.style.left = rightRect.x;
            RightContentVisualElement.style.top = rightRect.y;
            RightContentVisualElement.style.width = rightRect.width;
            RightContentVisualElement.style.height = rightRect.height;

            GUILayout.BeginArea(rightRect);
            OnRightGUI(new Rect(Vector2.zero, rightRect.size));
            GUILayout.EndArea();
        }

        protected virtual void OnLeftGUI(Rect rect)
        {
            if (MenuTreeView != null)
            {
                MenuTreeView.searchString = MenuTreeViewSearchField.OnGUI(MenuTreeView.searchString);
                leftScroll.y = 0;
                leftScroll = GUILayout.BeginScrollView(leftScroll);

                if (MenuTreeViewSearchField.HasFocus() && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
                {
                    MenuTreeView.SetFocusAndEnsureSelectedItem();
                    Event.current.Use();
                }

                var treeviewRect = EditorGUILayout.GetControlRect(false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                EditorGUI.DrawRect(treeviewRect, new Color(0.5f, 0.5f, 0.5f, 1));
                MenuTreeView.OnGUI(treeviewRect);
                if (MenuTreeView.HasSelection() && MenuTreeView.HasFocus() && Event.current.keyCode == KeyCode.F)
                {
                    MenuTreeView.searchString = string.Empty;
                    MenuTreeView.FrameItem(MenuTreeView.GetSelection()[0]);
                }

                GUILayout.EndScrollView();
            }
        }

        protected virtual void OnRightGUI(Rect rect)
        {
        }
    }
}
#endif