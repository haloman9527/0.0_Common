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
using UnityTreeView = UnityEditor.IMGUI.Controls.TreeView;
using UnityTreeViewItem = UnityEditor.IMGUI.Controls.TreeViewItem;

namespace CZToolKitEditor.IMGUI.Controls
{
    public class TreeViewItem : UnityTreeViewItem
    {
        public object userData;

        public new List<UnityTreeViewItem> children
        {
            get
            {
                if (base.children == null)
                    base.children = new List<UnityTreeViewItem>();
                return base.children;
            }
        }

        public void ClearChildren()
        {
            if (base.children != null)
            {
                base.children.Clear();
            }
        }
    }

    public class TreeView : UnityTreeView
    {
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

        private UnityTreeViewItem root;
        private TreeViewItemPool itemPool;

        public Action<IList<int>> onSelectionChanged;
        public Action onKeyEvent;
        public Action onContextClicked;
        public Action<TreeViewItem> onItemContextClicked;
        public Action<TreeViewItem> onItemSingleClicked;
        public Action<TreeViewItem> onItemDoubleClicked;
        public Action<TreeViewItem, string, string> renameEnded;

        public Action<RowGUIArgsBridge> onItemRowGUI;
        public Func<TreeViewItem, bool> canRename;
        public Func<TreeViewItem, bool> canMultiSelect;
        public Func<TreeViewItem, bool> canBeParent;

        public class TreeViewItemPool : ObjectPool<TreeViewItem>
        {
            protected override TreeViewItem Create()
            {
                return new TreeViewItem();
            }

            protected override void OnRecycle(TreeViewItem unit)
            {
                unit.userData = null;
                unit.displayName = string.Empty;
                unit.icon = null;
                unit.parent = null;
                unit.ClearChildren();
                base.OnRecycle(unit);
            }
        }

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

        public UnityTreeViewItem RootItem
        {
            get
            {
                if (root == null)
                {
                    root = new TreeViewItem() { id = -1, depth = -1, displayName = "Root" };
                }

                if (root.children == null)
                {
                    root.children = new List<UnityTreeViewItem>();
                }

                return root;
            }
        }

        public TreeView(TreeViewState state) : base(state)
        {
            this.itemPool = new TreeViewItemPool();
            onItemRowGUI += DefaultRowGUI;
        }

        public TreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {
            this.itemPool = new TreeViewItemPool();
            onItemRowGUI += DefaultRowGUI;
        }

        public TreeView(TreeViewState state, TreeViewItemPool itemPool) : base(state)
        {
            this.itemPool = itemPool;
            onItemRowGUI += DefaultRowGUI;
        }

        public TreeView(TreeViewState state, MultiColumnHeader multiColumnHeader, TreeViewItemPool itemPool) : base(state, multiColumnHeader)
        {
            this.itemPool = itemPool;
            onItemRowGUI += DefaultRowGUI;
        }

        public void Sort(Func<UnityTreeViewItem, UnityTreeViewItem, int> comparer)
        {
            Sort(RootItem as TreeViewItem);
            Reload();

            void Sort(TreeViewItem item)
            {
                item.children.QuickSort(comparer);
                foreach (var child in item.children)
                {
                    Sort(child as TreeViewItem);
                }
            }
        }

        protected override UnityTreeViewItem BuildRoot()
        {
            SetupDepthsFromParentsAndChildren(RootItem);
            return RootItem;
        }

        protected override bool CanMultiSelect(UnityTreeViewItem item)
        {
            if (canMultiSelect == null)
                return false;
            return canMultiSelect(item as TreeViewItem);
        }

        protected override bool CanRename(UnityTreeViewItem item)
        {
            if (canRename == null)
                return false;
            return canRename(item as TreeViewItem);
        }

        protected override bool CanBeParent(UnityTreeViewItem item)
        {
            if (canBeParent == null)
                return base.CanBeParent(item);
            return canBeParent(item as TreeViewItem);
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
            var item = args.item as TreeViewItem;

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

        public TreeViewItem AddMenuItem(string path, char separator = '/')
        {
            return AddMenuItem(path, (Texture2D)null, separator);
        }

        public TreeViewItem AddMenuItem(string path, Texture2D icon, char separator = '/')
        {
            if (string.IsNullOrEmpty(path))
                return null;

            var parent = RootItem as TreeViewItem;
            var p = path.Split(separator);
            if (p.Length > 1)
            {
                for (int i = 0; i < p.Length - 1; i++)
                {
                    var tempParent = parent.children.Find(item => item.displayName == p[i]) as TreeViewItem;
                    if (tempParent == null)
                    {
                        tempParent = itemPool.Spawn();
                        tempParent.id = GenerateID();
                        tempParent.displayName = p[i];
                        tempParent.parent = parent;
                        parent.children.Add(tempParent);
                    }

                    parent = tempParent;
                }
            }

            var item = itemPool.Spawn();
            item.icon = icon;
            item.id = GenerateID();
            item.displayName = p[p.Length - 1];
            item.parent = parent;
            parent.children.Add(item);
            return item;
        }

        public void Remove(TreeViewItem treeViewItem)
        {
            if (treeViewItem == null || treeViewItem.parent == null)
                return;
            treeViewItem.parent.children.Remove(treeViewItem);
        }

        public TreeViewItem FindItem(int id)
        {
            return FindItem(id, rootItem) as TreeViewItem;
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);
            onSelectionChanged?.Invoke(selectedIds);
        }

        public IEnumerable<TreeViewItem> Items()
        {
            return Foreach(RootItem as TreeViewItem);

            IEnumerable<TreeViewItem> Foreach(TreeViewItem parent)
            {
                foreach (var item in parent.children)
                {
                    yield return item as TreeViewItem;
                    foreach (var child in Foreach(item as TreeViewItem))
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
            var item = FindItem(id);
            onItemSingleClicked?.Invoke(item);
        }

        protected override void DoubleClickedItem(int id)
        {
            base.DoubleClickedItem(id);
            var item = FindItem(id);
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
            var item = FindItem(id);
            onItemContextClicked?.Invoke(item);
        }

        public void Clear()
        {
            foreach (var item in Items())
            {
                itemPool.Recycle(item);
            }

            RootItem.children.Clear();
            id = 0;
        }

        public void Dispose()
        {
            RootItem.children.Clear();
            id = 0;
        }

        private int id = 0;

        private int GenerateID()
        {
            return id++;
        }

        public struct RowGUIArgsBridge
        {
            /// <summary>
            ///   <para>Item for the current row being handled in TreeView.RowGUI.</para>
            /// </summary>
            public UnityTreeViewItem item;

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