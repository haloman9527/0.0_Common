#if UNITY_EDITOR
using Moyo.UnityEditors.IMGUI.Controls;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;
using TreeView = Moyo.UnityEditors.IMGUI.Controls.TreeView;

namespace Moyo.UnityEditors
{
    using TreeView = IMGUI.Controls.TreeView;

    public abstract class MenuEditorWindow : EditorWindow
    {
        public bool showMenu = true;
        public ResizableArea resizableArea;
        private VisualElement rightContentVisualElement;
        private Vector2 leftScroll;
        private Rect rightRect;
        private Rect sideRect;
        private Rect contentRect;

        private SearchField menuTreeViewSearchField;
        private TreeView menuTreeView;
        private TreeViewState menuTreeViewState;

        public abstract TreeView MenuTreeView { get; }

        public VisualElement RightContentVisualElement => rightContentVisualElement;

        protected virtual void OnEnable()
        {
            menuTreeViewSearchField = new SearchField();
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

            rightContentVisualElement = new VisualElement();
            rightContentVisualElement.pickingMode = PickingMode.Ignore;
            rootVisualElement.Add(rightContentVisualElement);
        }

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
                    resizableArea.rect.position = r.position;
                    resizableArea.maxSize = r.size;
                    resizableArea.rect.height = r.height;
                }

                resizableArea.OnGUI();

                sideRect = resizableArea.rect;
                sideRect.x += sideRect.width;
                sideRect.width = 1;

                rightRect = sideRect;
                rightRect.x += rightRect.width + 1;
                rightRect.width = position.width - resizableArea.rect.width - sideRect.width - 2;
                rightRect.width = Mathf.Max(rightRect.width, 100);
                rightRect.xMin += 10;
                rightRect.xMax -= 10;

                using (new GUILayout.AreaScope(resizableArea.rect))
                {
                    OnLeftGUI(new Rect(Vector2.zero, resizableArea.rect.size));
                }

                EditorGUI.DrawRect(sideRect, new Color(0.5f, 0.5f, 0.5f, 1));
            }
            else
            {
                rightRect = new Rect(Vector2.zero, position.size);
                rightRect.xMin += 11;
                rightRect.xMax -= 11;
            }

            rightContentVisualElement.style.position = Position.Relative;
            rightContentVisualElement.style.left = rightRect.x;
            rightContentVisualElement.style.top = rightRect.y;
            rightContentVisualElement.style.width = rightRect.width;
            rightContentVisualElement.style.height = rightRect.height;
            
            using (new GUILayout.AreaScope(rightRect))
            {
                OnRightGUI(new Rect(Vector2.zero, rightRect.size));
            }
        }

        protected virtual void OnLeftGUI(Rect rect)
        {
            if (MenuTreeView != null)
            {
                MenuTreeView.searchString = menuTreeViewSearchField.OnGUI(MenuTreeView.searchString);
                leftScroll.y = 0;
                leftScroll = GUILayout.BeginScrollView(leftScroll);

                if (menuTreeViewSearchField.HasFocus() && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
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