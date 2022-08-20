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
    public abstract class BasicMenuEditorWindow : EditorWindow
    {
        static readonly Dictionary<UnityObject, Editor> EditorCache = new Dictionary<UnityObject, Editor>();

        [SerializeField]
        readonly ResizableArea leftArea = new ResizableArea();

        string searchText = "";
        SearchField searchField;
        TreeViewState treeViewState = new TreeViewState();
        VisualElement rightRoot;

        Rect center;
        Rect leftRect = new Rect(0, 0, 150, 150);
        Rect rightRect;
        Vector2 rightScroll;

        public CZTreeView MenuTreeView { get; private set; }

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
        protected Rect LeftRect { get { return leftRect; } }
        protected virtual float RightMinWidth { get; set; } = 500;
        protected Rect RightRect { get { return rightRect; } }

        protected virtual void OnEnable()
        {
            leftArea.minSize = new Vector2(LeftMinWidth, 50);
            leftArea.side = 10;
            leftArea.SetEnable(ResizableArea.UIDirection.Right, true);

            searchField = new SearchField();
            RefreshTreeView();
        }

        protected abstract CZTreeView BuildMenuTree(TreeViewState treeViewState);

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
            var tempCenter = EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            if (Event.current.type != EventType.Layout && Event.current.type != EventType.Used)
            {
                center = tempCenter;
                leftRect.x = center.x;
                leftRect.y = center.y;
                leftRect.height = center.height;
                leftArea.maxSize = center.size;
            }

            leftRect = leftArea.OnGUI(leftRect);
            // 左列表
            using (var leftScope = new GUILayout.AreaScope(leftRect))
            {
                GUILayout.Space(3);
                OnLeftGUI();
            }

            // 分割线
            Rect sideRect = center;
            sideRect.xMin = leftRect.xMax;
            sideRect.width = 1;
            EditorGUI.DrawRect(sideRect, new Color(0.5f, 0.5f, 0.5f, 1));

            rightRect = center;
            rightRect.xMin = sideRect.xMax + 2;
            rightRect.width = Mathf.Max(rightRect.width, RightMinWidth);

            RightRoot.style.left = rightRect.xMin + 50;
            RightRoot.style.width = rightRect.width - 100;
            RightRoot.style.top = rightRect.yMin;
            RightRoot.style.height = rightRect.height;

            // 右绘制
            using (var rightScope = new GUILayout.AreaScope(rightRect))
            {
                rightScroll = GUILayout.BeginScrollView(rightScroll, false, false);
                OnRightGUI(MenuTreeView.GetSelection());
                GUILayout.EndScrollView();
            }

            EditorGUILayout.EndVertical();
        }

        protected virtual void OnSerachFieldGUI(string placeholderText = "")
        {
            var searchFieldRect = EditorGUILayout.GetControlRect(GUILayout.Height(20), GUILayout.ExpandWidth(true));
            string tempSearchText = searchField.OnGUI(searchFieldRect, searchText);
            if (tempSearchText != searchText)
            {
                searchText = tempSearchText;
                MenuTreeView.searchString = searchText;
            }
            if (searchField.HasFocus() && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
            {
                MenuTreeView.SetFocusAndEnsureSelectedItem();
                Event.current.Use();
            }

            if (string.IsNullOrEmpty(searchText))
            {
                EditorGUI.BeginDisabledGroup(true);

                Rect placeholderRect = searchFieldRect;
                placeholderRect.xMin += 15;
                placeholderRect.yMin -= 3;
                placeholderRect.yMax -= 3;
                GUI.Label(placeholderRect, placeholderText);

                EditorGUI.EndDisabledGroup();
            }
        }

        protected virtual void OnTreeViewGUI()
        {
            var treeViewRect = EditorGUILayout.GetControlRect(GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(treeViewRect, new Color(0.5f, 0.5f, 0.5f, 1));
            if (MenuTreeView != null)
                MenuTreeView.OnGUI(treeViewRect);
        }

        protected virtual void OnLeftGUI()
        {
            EditorGUILayout.BeginVertical();

            OnSerachFieldGUI();
            OnTreeViewGUI();

            EditorGUILayout.EndVertical();
        }

        protected virtual void OnRightGUI(IList<int> selection)
        {
            if (selection.Count == 0)
                return;
            CZTreeViewItem first = MenuTreeView.FindItem(selection[0]);
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
            }
        }
    }
}
#endif