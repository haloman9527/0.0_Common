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

        string searchText = "";
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
            Rect center = EditorGUILayout.BeginVertical();

            if (Event.current.type == EventType.Repaint)
            {
                resizableAreaRect.x = center.x;
                resizableAreaRect.y = center.y;
                resizableAreaRect.height = center.height;
                resizableArea.maxSize = center.size;
            }

            resizableAreaRect = resizableArea.OnGUI(resizableAreaRect);
            // 左列表
            using (var centerArea = new GUILayout.AreaScope(resizableAreaRect))
            {
                GUILayout.Space(3);
                OnLeftGUI(resizableAreaRect);
            }

            // 分割线
            Rect sideRect = resizableAreaRect;
            sideRect.x += sideRect.width;
            sideRect.width = 1;
            EditorGUI.DrawRect(sideRect, new Color(0.5f, 0.5f, 0.5f, 1));

            rightRect = sideRect;
            rightRect.x += rightRect.width + 1;
            rightRect.width = center.width - resizableAreaRect.width - sideRect.width - 2;
            rightRect.width = Mathf.Max(rightRect.width, RightMinWidth);

            RightRoot.style.left = rightRect.xMin + 50;
            RightRoot.style.width = rightRect.width - 100;
            RightRoot.style.top = rightRect.yMin;
            RightRoot.style.height = rightRect.height;

            // 右绘制
            GUILayoutUtility.GetRect(rightRect.width, rightRect.height);
            using (var rightArea = new GUILayout.AreaScope(rightRect))
            {
                rightScroll = GUILayout.BeginScrollView(rightScroll, false, false);
                OnRightGUI(MenuTreeView.GetSelection());
                GUILayout.EndScrollView();
            }

            EditorGUILayout.EndVertical();
        }

        protected virtual void OnLeftGUI(Rect leftRect)
        {
            var searchFieldRect = EditorGUILayout.GetControlRect(GUILayout.Height(20), GUILayout.Width(leftRect.width - 2));
            string tempSearchText = searchField.OnGUI(searchFieldRect, searchText);
            if (tempSearchText != searchText)
            {
                searchText = tempSearchText;
                MenuTreeView.searchString = searchText;
            }

            var treeViewRect = EditorGUILayout.GetControlRect(GUILayout.ExpandHeight(true), GUILayout.Width(leftRect.width - 2));
            EditorGUI.DrawRect(treeViewRect, new Color(0.5f, 0.5f, 0.5f, 1));
            MenuTreeView.OnGUI(treeViewRect);
            EditorGUILayout.GetControlRect(GUILayout.Height(treeViewRect.height));
        }

        protected virtual void OnRightGUI(IList<int> selection)
        {
            if (selection.Count == 0) return;
            CZMenuTreeViewItem first = MenuTreeView.FindItem(selection[0]) as CZMenuTreeViewItem;
            if (first == null) return;

            switch (first.userData)
            {
                case null:
                    GUILayout.Space(10);
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label(first.displayName, (GUIStyle)"AM MixerHeader2");
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

        protected virtual Rect GetTreeViewRect(Rect treeViewRect)
        {
            return treeViewRect;
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