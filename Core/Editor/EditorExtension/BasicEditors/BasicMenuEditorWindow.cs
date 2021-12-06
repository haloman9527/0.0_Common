#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 半只龙虾人
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */
#endregion
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;
using UnityObject = UnityEngine.Object;

namespace CZToolKit.Core.Editors
{
    [Serializable]
    public abstract class BasicMenuEditorWindow : BasicEditorWindow
    {
        static readonly Dictionary<UnityObject, Editor> EditorCache = new Dictionary<UnityObject, Editor>();

        [SerializeField]
        ResizableArea resizableArea = new ResizableArea();
        protected Rect resizableAreaRect = new Rect(0, 0, 150, 150);

        string searchText;
        SearchField searchField;
        TreeViewState treeViewState = new TreeViewState();
        VisualElement rightRoot;

        Vector2 scroll;
        Rect rightRect;
        Vector2 rightScroll;

        public CZMenuTreeView MenuTreeView { get; private set; }

        protected VisualElement RightRoot
        {
            get
            {
                if (rightRoot == null)
                {
                    rightRoot = new VisualElement();
                    rootVisualElement.Add(rightRoot);
                }
                return rightRoot;
            }
        }
        protected float LeftMinWidth { get; set; } = 50;
        protected float RightMinWidth { get; set; } = 500;
        protected Rect RightRect { get { return rightRect; } }

        protected virtual void OnEnable()
        {
            resizableArea.minSize = new Vector2(LeftMinWidth, 50);
            resizableArea.side = 10;
            resizableArea.SetEnable(ResizableArea.UIDirection.Right, true);

            searchField = new SearchField();
            RefreshTreeView();
        }

        protected abstract CZMenuTreeView BuildMenuTree(TreeViewState treeViewState);

        public void RefreshTreeView()
        {
            MenuTreeView = BuildMenuTree(treeViewState);
            MenuTreeView.Reload();
            var selection = MenuTreeView.GetSelection();
            OnSelectionChanged(selection);
            MenuTreeView.onSelectionChanged += OnSelectionChanged;
        }

        protected virtual void OnSelectionChanged(IList<int> selection) { }

        protected virtual void OnGUI()
        {
            resizableArea.maxSize = position.size;
            resizableAreaRect.height = position.height;
            resizableAreaRect = resizableArea.OnGUI(resizableAreaRect);

            GUILayout.BeginArea(resizableAreaRect);
            scroll.y = 0;
            scroll = GUILayout.BeginScrollView(scroll);

            Rect searchFieldRect = resizableAreaRect;
            searchFieldRect.height = 22;
            searchFieldRect.y += 3;
            searchFieldRect.x += 5;
            searchFieldRect.width -= 10;
            searchFieldRect.width = Mathf.Max(100, searchFieldRect.width);

            string tempSearchText = searchField.OnGUI(searchFieldRect, searchText);
            if (tempSearchText != searchText)
            {
                searchText = tempSearchText;
                MenuTreeView.searchString = searchText;
            }

            Rect treeviewRect = resizableAreaRect;
            treeviewRect.y += searchFieldRect.height;
            treeviewRect.height -= searchFieldRect.height;
            EditorGUI.DrawRect(treeviewRect, new Color(0.5f, 0.5f, 0.5f, 1));
            MenuTreeView.OnGUI(treeviewRect);
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            Rect sideRect = resizableAreaRect;
            sideRect.x += sideRect.width;
            sideRect.width = 1;
            EditorGUI.DrawRect(sideRect, new Color(0.5f, 0.5f, 0.5f, 1));

            rightRect = sideRect;
            rightRect.x += rightRect.width + 1;
            rightRect.width = position.width - resizableAreaRect.width - sideRect.width - 2;
            rightRect.width = Mathf.Max(rightRect.width, RightMinWidth);

            RightRoot.style.left = RightRect.xMin + 50;
            RightRoot.style.width = RightRect.width - 100;
            RightRoot.style.top = RightRect.yMin;
            RightRoot.style.height = RightRect.height;

            GUILayout.BeginArea(rightRect);
            rightRect.x = 0;
            rightRect.y = 0;

            IList<int> selection = MenuTreeView.GetSelection();
            if (selection.Count > 0)
            {
                rightScroll = GUILayout.BeginScrollView(rightScroll, false, false);
                OnRightGUI(MenuTreeView.FindItem(selection[0]) as CZMenuTreeViewItem);
                GUILayout.EndScrollView();
            }

            GUILayout.EndArea();
        }

        protected virtual void OnRightGUI(CZMenuTreeViewItem selectedItem)
        {
            if (selectedItem == null) return;
            switch (selectedItem.userData)
            {
                case null:
                    GUILayout.Space(10);
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label(selectedItem.displayName, (GUIStyle)"AM MixerHeader2");
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                    EditorGUI.DrawRect(GUILayoutUtility.GetRect(rightRect.width, 1), Color.gray);
                    break;
                case UnityObject unityObject:
                    if (unityObject == null) break;
                    if (!EditorCache.TryGetValue(unityObject, out Editor editor))
                        EditorCache[unityObject] = editor = Editor.CreateEditor(unityObject);
                    editor.OnInspectorGUI();
                    Repaint();
                    break;
                default:
                    break;
            }
        }
    }

    public class CZMenuTreeView : CZTreeView
    {
        public CZMenuTreeView(TreeViewState state) : base(state)
        {
            rowHeight = 30;
#if !UNITY_2019_1_OR_NEWER
            customFoldoutYOffset = rowHeight / 2 - 8;
#endif
        }

        public CZMenuTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {
            rowHeight = 30;
#if !UNITY_2019_1_OR_NEWER
            customFoldoutYOffset = rowHeight / 2 - 8;
#endif
        }

        public string GetParentPath(string _path)
        {
            int index = _path.LastIndexOf('/');
            if (index == -1)
                return null;
            return _path.Substring(0, index);
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            base.RowGUI(args);
            CZMenuTreeViewItem item = args.item as CZMenuTreeViewItem;
            if (item != null)
                item.itemDrawer?.Invoke(args.rowRect, item);
        }
    }

    public class CZMenuTreeViewItem : CZTreeViewItem
    {
        public CZMenuTreeViewItem() : base() { }
    }
}
#endif