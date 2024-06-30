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
 *  Github: https://github.com/haloman9527
 *  Blog: https://www.haloman.net/
 *
 */

#endregion

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using CZToolKit;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace CZToolKitEditor.IMGUI.Controls
{
    public class CZTreeViewItem : TreeViewItem
    {
        public object userData;

        public new List<TreeViewItem> children
        {
            get
            {
                if (base.children == null)
                    base.children = new List<TreeViewItem>();
                return base.children;
            }
        }
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

        private static void SplitMenuPath(string menuPath, char separator, out string path, out string name)
        {
            menuPath = menuPath.Trim(separator);
            int num = menuPath.LastIndexOf(separator);
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

        public Action<IList<int>> onSelectionChanged;
        public Action onKeyEvent;
        public Action onContextClicked;
        public Action<CZTreeViewItem> onItemContextClicked;
        public Action<CZTreeViewItem> onItemSingleClicked;
        public Action<CZTreeViewItem> onItemDoubleClicked;
        public Action<CZTreeViewItem, string, string> renameEnded;

        public Action<RowGUIArgsBridge> onItemRowGUI;
        public Func<CZTreeViewItem, bool> canRename;
        public Func<CZTreeViewItem, bool> canMultiSelect;
        public Func<CZTreeViewItem, bool> canBeParent;

        public float RowHeight
        {
            get => rowHeight;
            set => rowHeight = value;
        }

        public bool ShowBoder
        {
            get => showBorder;
            set => showBorder = value;
        }

        public bool ShowAlternatingRowBackgrounds
        {
            get => showAlternatingRowBackgrounds;
            set => showAlternatingRowBackgrounds = value;
        }

        public float DepthIndentWidth
        {
            get => this.depthIndentWidth;
        }

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

        public CZTreeView(TreeViewState state) : base(state)
        {
            onItemRowGUI += DefaultRowGUI;
        }

        public CZTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {
            onItemRowGUI += DefaultRowGUI;
        }

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

        protected override bool CanBeParent(TreeViewItem item)
        {
            if (canBeParent == null)
                return base.CanBeParent(item);
            return canBeParent(item as CZTreeViewItem);
        }

        protected override void RenameEnded(RenameEndedArgs args)
        {
            var item = FindItem(args.itemID);

            renameEnded?.Invoke(item, args.originalName, args.newName);
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var argsBridge = new RowGUIArgsBridge()
            {
                item = args.item,
                label = args.label,
                rowRect = args.rowRect,
                row = args.row,
                selected = args.selected,
                focused = args.focused,
                isRenaming = args.isRenaming
            };
            onItemRowGUI?.Invoke(argsBridge);
        }

        protected void DefaultRowGUI(RowGUIArgsBridge args)
        {
            var item = args.item as CZTreeViewItem;

            if (!args.isRenaming)
            {
                Rect labelRect = args.rowRect;
                if (hasSearch)
                    labelRect.xMin += depthIndentWidth;
                else
                    labelRect.xMin += item.depth * depthIndentWidth + depthIndentWidth;
                var label = EditorGUIUtility.TrTextContent(args.label);
                label.image = item.icon;
                GUI.Label(labelRect, label);
            }
        }

        public void AddMenuItem<T>(string path, T treeViewItem, char separator = '/') where T : CZTreeViewItem, new()
        {
            if (string.IsNullOrEmpty(path)) return;

            var parentItem = RootItem as CZTreeViewItem;

            SplitMenuPath(path, separator, out path, out string name);
            if (!string.IsNullOrEmpty(path))
            {
                string[] tmpPath = path.Split(separator);
                for (int i = 0; i < tmpPath.Length; i++)
                {
                    var tempItem = parentItem.children.Find(item => item.displayName == tmpPath[i]) as CZTreeViewItem;
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

        public T AddMenuItem<T>(string path, char separator = '/') where T : CZTreeViewItem, new()
        {
            return AddMenuItem<T>(path, (Texture2D)null, separator);
        }

        public T AddMenuItem<T>(string path, Texture2D icon, char separator = '/') where T : CZTreeViewItem, new()
        {
            if (string.IsNullOrEmpty(path))
                return null;
            T item = new T();
            item.icon = icon;
            AddMenuItem(path, item, separator);
            return item;
        }

        public void Remove(CZTreeViewItem treeViewItem)
        {
            if (treeViewItem == null || treeViewItem.parent == null)
                return;
            treeViewItem.parent.children.Remove(treeViewItem);
        }

        public CZTreeViewItem FindItem(int id)
        {
            return FindItem(id, rootItem) as CZTreeViewItem;
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
                    foreach (var child in Foreach(item as CZTreeViewItem))
                    {
                        yield return child;
                    }
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
            onItemSingleClicked?.Invoke(item);
        }

        protected override void DoubleClickedItem(int id)
        {
            base.DoubleClickedItem(id);
            CZTreeViewItem item = FindItem(id);
            onItemDoubleClicked?.Invoke(item);
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
            onItemContextClicked?.Invoke(item);
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

        public struct RowGUIArgsBridge
        {
            /// <summary>
            ///   <para>Item for the current row being handled in TreeView.RowGUI.</para>
            /// </summary>
            public TreeViewItem item;

            /// <summary>
            ///   <para>Label used for text rendering of the item displayName. Note this is an empty string when isRenaming == true.</para>
            /// </summary>
            public string label;

            /// <summary>
            ///   <para>Row rect for the current row being handled.</para>
            /// </summary>
            public Rect rowRect;

            /// <summary>
            ///   <para>Row index into the list of current rows.</para>
            /// </summary>
            public int row;

            /// <summary>
            ///   <para>This value is true when the current row's item is part of the current selection.</para>
            /// </summary>
            public bool selected;

            /// <summary>
            ///   <para>This value is true only when the TreeView has keyboard focus and the TreeView's window has focus.</para>
            /// </summary>
            public bool focused;

            /// <summary>
            ///   <para>This value is true when the ::item is currently being renamed.</para>
            /// </summary>
            public bool isRenaming;
        }
    }
}
#endif