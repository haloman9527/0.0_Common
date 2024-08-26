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

        private Dictionary<string, TreeViewItem> childrenMap;

        public new TreeViewItem parent
        {
            get => base.parent as TreeViewItem;
            set => base.parent = value;
        }

        public override List<UnityTreeViewItem> children
        {
            get
            {
                if (base.children == null)
                    base.children = new List<UnityTreeViewItem>();
                return base.children;
            }
        }

        public Dictionary<string, TreeViewItem> ChildrenMap
        {
            get
            {
                if (childrenMap == null)
                {
                    childrenMap = new Dictionary<string, TreeViewItem>();
                }

                return childrenMap;
            }
        }

        public override string displayName { get; set; }
    }

    public class TreeView : UnityTreeView
    {
        private TreeViewItem root;
        private TreeViewItemPool itemPool;
        private bool sharedItemPool;
        private Dictionary<int, TreeViewItem> itemMap = new Dictionary<int, TreeViewItem>();

        public Action<IList<int>> onSelectionChanged;
        public Action onKeyEvent;
        public Action onContextClicked;
        public Action<TreeViewItem> onItemContextClicked;
        public Action<TreeViewItem> onItemSingleClicked;
        public Action<TreeViewItem> onItemDoubleClicked;
        public Action<TreeViewItem, string, string> renameEnded;
        
        public Func<TreeViewItem, bool> canRename;
        public Func<TreeViewItem, bool> canMultiSelect;
        public Func<TreeViewItem, bool> canBeParent;
        
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
            set => this.depthIndentWidth = value;
        }

        public int ColumnIndexForTreeFoldouts
        {
            get => this.columnIndexForTreeFoldouts;
            set => this.columnIndexForTreeFoldouts = value;
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
            this.sharedItemPool = false;
            this.itemPool = new TreeViewItemPool();
        }

        public TreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {
            this.sharedItemPool = false;
            this.itemPool = new TreeViewItemPool();
        }

        public TreeView(TreeViewState state, TreeViewItemPool itemPool) : base(state)
        {
            this.sharedItemPool = true;
            this.itemPool = itemPool;
        }

        public TreeView(TreeViewState state, MultiColumnHeader multiColumnHeader, TreeViewItemPool itemPool) : base(state, multiColumnHeader)
        {
            this.sharedItemPool = true;
            this.itemPool = itemPool;
        }

        public void Sort(Func<UnityTreeViewItem, UnityTreeViewItem, int> comparer)
        {
            SortRecursive(RootItem as TreeViewItem);
            Reload();

            void SortRecursive(TreeViewItem item)
            {
                if (!item.hasChildren)
                    return;

                item.children.QuickSort(comparer);
                foreach (var child in item.children)
                {
                    SortRecursive(child as TreeViewItem);
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

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            onSelectionChanged?.Invoke(selectedIds);
        }

        protected override void KeyEvent()
        {
            onKeyEvent?.Invoke();
        }

        protected override void SingleClickedItem(int id)
        {
            var item = FindItem(id);
            onItemSingleClicked?.Invoke(item);
        }

        protected override void DoubleClickedItem(int id)
        {
            var item = FindItem(id);
            onItemDoubleClicked?.Invoke(item);
        }

        protected override void ContextClicked()
        {
            onContextClicked?.Invoke();
        }

        protected override void ContextClickedItem(int id)
        {
            var item = FindItem(id);
            if (item == null)
                return;

            onItemContextClicked?.Invoke(item);
        }

        protected sealed override void RowGUI(RowGUIArgs args)
        {
            ItemRowGUI(args.item as TreeViewItem, args);
        }

        protected virtual void ItemRowGUI(TreeViewItem item, RowGUIArgs args)
        {
            DefaultRowGUI(args);
        }

        protected void DefaultRowGUI(RowGUIArgs args)
        {
            if (!args.isRenaming)
            {
                var item = args.item;
                var labelRect = args.rowRect;
                if (hasSearch)
                    labelRect.xMin += depthIndentWidth;
                else
                    labelRect.xMin += item.depth * depthIndentWidth + depthIndentWidth;
                var label = EditorGUIUtility.TrTextContent(args.label);
                label.image = item.icon;
                GUI.Label(labelRect, label);
            }
        }

        public TreeViewItem AddMenuItem(string path, char separator = '/', bool split = true)
        {
            return AddMenuItemTo(RootItem as TreeViewItem, path, null, separator, split);
        }

        public TreeViewItem AddMenuItem(string path, Texture2D icon, char separator = '/', bool split = true)
        {
            return AddMenuItemTo(RootItem as TreeViewItem, path, icon, separator, split);
        }

        public TreeViewItem AddMenuItemTo(TreeViewItem parent, string path, char separator = '/', bool split = true)
        {
            return AddMenuItemTo(parent, path, null, separator, split);
        }

        public TreeViewItem AddMenuItemTo(TreeViewItem parent, string path, Texture2D icon, char separator = '/', bool split = true)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            var name = path;

            if (split && path.IndexOf(separator) != -1)
            {
                var p = path.Split(separator);
                name = p[^1];
                for (int i = 0; i < p.Length - 1; i++)
                {
                    if (!parent.ChildrenMap.TryGetValue(p[i], out var tmpParent))
                    {
                        tmpParent = itemPool.Spawn();
                        tmpParent.id = GenerateID();
                        tmpParent.displayName = p[i];
                        tmpParent.parent = parent;
                        tmpParent.depth = parent.depth + 1;
                        parent.children.Add(tmpParent);
                        parent.ChildrenMap[p[i]] = tmpParent;
                        itemMap[tmpParent.id] = tmpParent;
                    }

                    parent = tmpParent;
                }
            }

            var item = itemPool.Spawn();
            item.icon = icon;
            item.id = GenerateID();
            item.displayName = name;
            item.parent = parent;
            item.depth = parent.depth + 1;
            parent.children.Add(item);
            parent.ChildrenMap[name] = item;
            itemMap[item.id] = item;
            return item;
        }

        public void Remove(TreeViewItem treeViewItem)
        {
            if (treeViewItem == null || treeViewItem.parent == null)
                return;

            itemMap.Remove(treeViewItem.id);
            treeViewItem.parent.children.Remove(treeViewItem);
        }

        public TreeViewItem FindItem(int id)
        {
            if (itemMap.TryGetValue(id, out var item))
            {
                return item;
            }

            return FindItem(id, rootItem) as TreeViewItem;
        }

        public TreeViewItem FindItem(string path, char separator = '/', bool split = true)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            var parent = RootItem as TreeViewItem;
            var result = (TreeViewItem)null;
            if (split && path.IndexOf(separator) != -1)
            {
                var p = path.Split(separator);
                for (int i = 0; i < p.Length; i++)
                {
                    if (!parent.ChildrenMap.TryGetValue(p[i], out result))
                    {
                        break;
                    }

                    parent = result;
                }
            }
            else
            {
                parent.ChildrenMap.TryGetValue(path, out result);
            }

            return result;
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

        public void Clear()
        {
            foreach (var item in Items())
            {
                itemPool.Recycle(item);
            }

            itemMap.Clear();
            root.userData = null;
            if (root.hasChildren)
            {
                root.children.Clear();
                root.ChildrenMap.Clear();
            }
            lastItemId = 0;
        }

        public void Dispose()
        {
            if (sharedItemPool)
            {
                foreach (var item in Items())
                {
                    itemPool.Recycle(item);
                }
            }

            itemMap.Clear();
            root.userData = null;
            if (root.hasChildren)
            {
                root.children.Clear();
                root.ChildrenMap.Clear();
            }
            lastItemId = 0;
        }

        private int lastItemId = 0;

        private int GenerateID()
        {
            return lastItemId++;
        }

        public class TreeViewItemPool : BaseObjectPool<TreeViewItem>
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
                if (unit.hasChildren)
                {
                    unit.children.Clear();
                    unit.ChildrenMap.Clear();
                }
                base.OnRecycle(unit);
            }
        }
    }
}
#endif