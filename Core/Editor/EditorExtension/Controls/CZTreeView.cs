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

namespace CZToolKit.Core.Editors
{
    public class CZTreeViewItem : TreeViewItem
    {
        public object userData;
        public Action<Rect> itemDrawer;
        public Action onContextClicked;
        public Action onDoubleClicked;

        public new List<TreeViewItem> children
        {
            get
            {
                if (base.children == null)
                    base.children = new List<TreeViewItem>();
                return base.children;
            }
        }

        public CZTreeViewItem() { }
    }

    public class CZTreeView : TreeView
    {
        public class Styles
        {
            public GUIStyle leftLabelStyle;

            public Styles()
            {
                leftLabelStyle = new GUIStyle(EditorStyles.label);
                leftLabelStyle.alignment = TextAnchor.MiddleLeft;
            }
        }

        private static void SplitMenuPath(string menuPath, out string path, out string name)
        {
            menuPath = menuPath.Trim('/');
            int num = menuPath.LastIndexOf('/');
            if (num == -1)
            {
                path = "";
                name = menuPath;
                return;
            }
            path = menuPath.Substring(0, num);
            name = menuPath.Substring(num + 1);
        }

        TreeViewItem root;
        Styles styles;

        public event Action<IList<int>> onSelectionChanged;
        public event Action onKeyEvent;
        public event Action onContextClicked;
        public event Action<CZTreeViewItem> onContextClickedItem;
        public event Action<CZTreeViewItem> onSingleClickedItem;
        public event Action<CZTreeViewItem> onDoubleClickedItem;
        public event Func<CZTreeViewItem, bool> canRename;
        public event Func<CZTreeViewItem, bool> canMultiSelect;

        public Styles GUIStyles
        {
            get
            {
                if (styles == null) styles = new Styles();
                return styles;
            }
            set { styles = value; }
        }
        public float RowHeight { get => rowHeight; set => rowHeight = value; }
        public bool ShowBoder { get => showBorder; set => showBorder = value; }
        public bool ShowAlternatingRowBackgrounds { get => showAlternatingRowBackgrounds; set => showAlternatingRowBackgrounds = value; }
        public TreeViewItem RootItem
        {
            get
            {
                if (root == null)
                {
                    root = new CZTreeViewItem() { id = -1, depth = -1, displayName = "Root" };
                }
                if (root.children == null)
                {
                    root.children = new List<TreeViewItem>();
                }
                return root;
            }
        }

        public CZTreeView(TreeViewState state) : base(state) { }

        public CZTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader) { }

        protected override TreeViewItem BuildRoot()
        {
            SetupDepthsFromParentsAndChildren(RootItem);
            return RootItem;
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            if (canMultiSelect == null)
                return false;
            return canMultiSelect(item as CZTreeViewItem);
        }

        protected override bool CanRename(TreeViewItem item)
        {
            if (canRename == null)
                return false;
            return canRename(item as CZTreeViewItem);
        }

        protected override void RenameEnded(RenameEndedArgs args)
        {

        }

        protected override void RowGUI(RowGUIArgs args)
        {
            CZTreeViewItem item = args.item as CZTreeViewItem;

            if (!args.isRenaming)
            {
                Rect labelRect = args.rowRect;
                if (hasSearch)
                    labelRect.xMin += depthIndentWidth;
                else
                    labelRect.xMin += item.depth * depthIndentWidth + depthIndentWidth;
                GUIContent textContent = EditorGUIUtility.TrTextContent(item.displayName);
                textContent.image = item.icon;
                GUI.Label(labelRect, textContent, GUIStyles.leftLabelStyle);

                if (item != null)
                    item.itemDrawer?.Invoke(args.rowRect);
            }
        }

        public void AddMenuItem<T>(string path, T treeViewItem) where T : CZTreeViewItem, new()
        {
            if (string.IsNullOrEmpty(path)) return;

            CZTreeViewItem parentItem = RootItem as CZTreeViewItem;

            SplitMenuPath(path, out path, out string name);
            if (!string.IsNullOrEmpty(path))
            {
                string[] tmpPath = path.Split('/');
                for (int i = 0; i < tmpPath.Length; i++)
                {
                    CZTreeViewItem tempItem = parentItem.children.Find(item => item.displayName == tmpPath[i]) as CZTreeViewItem;
                    if (tempItem != null)
                    {
                        parentItem = tempItem;
                    }
                    else
                    {
                        tempItem = new T() { id = GenerateID(), displayName = tmpPath[i], parent = parentItem };
                        parentItem.children.Add(tempItem);
                        parentItem = tempItem;
                    }
                }
            }

            treeViewItem.id = GenerateID();
            treeViewItem.displayName = name;
            treeViewItem.parent = parentItem;
            parentItem.children.Add(treeViewItem);
        }

        public T AddMenuItem<T>(string _path) where T : CZTreeViewItem, new()
        {
            return AddMenuItem<T>(_path, (Texture2D)null);
        }

        public T AddMenuItem<T>(string _path, Texture2D _icon) where T : CZTreeViewItem, new()
        {
            if (string.IsNullOrEmpty(_path))
                return null;
            T item = new T();
            item.icon = _icon;
            AddMenuItem(_path, item);
            return item;
        }

        public void Remove(CZTreeViewItem treeViewItem)
        {
            if (treeViewItem == null || treeViewItem.parent == null)
                return;
            treeViewItem.parent.children.Remove(treeViewItem);
        }

        public CZTreeViewItem FindItem(int _id)
        {
            return FindItem(_id, rootItem) as CZTreeViewItem;
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);
            onSelectionChanged?.Invoke(selectedIds);
        }

        public IEnumerable<CZTreeViewItem> Items()
        {
            return Foreach(RootItem as CZTreeViewItem);

            IEnumerable<CZTreeViewItem> Foreach(CZTreeViewItem parent)
            {
                foreach (var item in parent.children)
                {
                    yield return item as CZTreeViewItem;
                }
            }
        }

        protected override void KeyEvent()
        {
            base.KeyEvent();
            onKeyEvent?.Invoke();
        }

        protected override void SingleClickedItem(int id)
        {
            base.SingleClickedItem(id);
            CZTreeViewItem item = FindItem(id);
            onSingleClickedItem?.Invoke(item);
        }

        protected override void DoubleClickedItem(int id)
        {
            base.DoubleClickedItem(id);
            CZTreeViewItem item = FindItem(id);
            onDoubleClickedItem?.Invoke(item);
            item.onDoubleClicked?.Invoke();
        }

        protected override void ContextClicked()
        {
            base.ContextClicked();
            onContextClicked?.Invoke();
        }

        protected override void ContextClickedItem(int id)
        {
            base.ContextClickedItem(id);
            CZTreeViewItem item = FindItem(id);
            onContextClickedItem?.Invoke(item);
            item.onContextClicked?.Invoke();
        }

        public void Clear()
        {
            RootItem.children.Clear();
            id = 0;
        }


        int id = 0;
        int GenerateID()
        {
            return id++;
        }

        public void IDAlloc(CZTreeViewItem item)
        {
            item.id = GenerateID();
        }
    }
}
#endif