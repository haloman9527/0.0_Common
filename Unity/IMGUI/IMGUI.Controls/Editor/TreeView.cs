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

        public IReadOnlyDictionary<string, TreeViewItem> ChildrenMap
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

        public void Clear()
        {
            this.userData = null;

            if (this.hasChildren)
            {
                this.children.Clear();
            }

            if (this.childrenMap != null)
            {
                this.childrenMap.Clear();
            }
        }

        public void Reset()
        {
            this.userData = null;
            this.displayName = string.Empty;
            this.icon = null;
            this.parent = null;

            if (this.hasChildren)
            {
                this.children.Clear();
            }

            if (this.childrenMap != null)
            {
                this.childrenMap.Clear();
            }
        }

        public new void AddChild(UnityTreeViewItem item)
        {
            throw new Exception("禁用此行为");
        }

        public void AddChild(TreeViewItem item)
        {
            this.children.Add(item);
            if (childrenMap == null)
            {
                childrenMap = new Dictionary<string, TreeViewItem>();
            }

            childrenMap[item.displayName] = item;
            item.parent = this;
            item.depth = this.depth + 1;
        }

        public void InsertChild(int index, TreeViewItem item)
        {
            this.children.Insert(index, item);
            if (childrenMap == null)
            {
                childrenMap = new Dictionary<string, TreeViewItem>();
            }

            childrenMap[item.displayName] = item;
            item.parent = this;
            item.depth = this.depth + 1;
        }

        public void RemoveChild(TreeViewItem item)
        {
            if (!children.Remove(item))
            {
                return;
            }

            if (childrenMap != null && childrenMap.TryGetValue(item.displayName, out var child) && child == item)
            {
                childrenMap.Remove(item.displayName);
            }

            item.parent = null;
        }
    }

    public class TreeView<T> : UnityTreeView where T : TreeViewItem, new()
    {
        private T root;
        private TreeViewItemPool itemPool;
        private bool sharedItemPool;
        private Dictionary<int, T> itemMap = new Dictionary<int, T>();

        public Action<IList<int>> onSelectionChanged;
        public Action onKeyEvent;
        public Action onContextClicked;
        public Action<T> onItemContextClicked;
        public Action<T> onItemSingleClicked;
        public Action<T> onItemDoubleClicked;
        public Action<T, string, string> renameEnded;

        public Func<T, bool> canRename;
        public Func<T, bool> canMultiSelect;
        public Func<T, bool> canBeParent;

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
                    root = new T() { id = -1, depth = -1, displayName = "Root" };
                }

                if (root.children == null)
                {
                    root.children = new List<UnityTreeViewItem>();
                }

                return root;
            }
        }

        public T Root => (T)RootItem;

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
            SortRecursive(RootItem as T);
            Reload();

            void SortRecursive(T item)
            {
                if (!item.hasChildren)
                    return;

                item.children.QuickSort(comparer);
                foreach (var child in item.children)
                {
                    SortRecursive(child as T);
                }
            }
        }

        protected override UnityTreeViewItem BuildRoot()
        {
            SetupDepthsFromParentsAndChildren(RootItem);
            return RootItem;
        }

        protected sealed override bool DoesItemMatchSearch(UnityTreeViewItem item, string search)
        {
            return DoesItemMatchSearch(item as T, search);
        }

        protected virtual bool DoesItemMatchSearch(T item, string search)
        {
            return base.DoesItemMatchSearch(item, search);
        }

        protected override bool CanMultiSelect(UnityTreeViewItem item)
        {
            if (canMultiSelect == null)
                return false;
            return canMultiSelect(item as T);
        }

        protected override bool CanRename(UnityTreeViewItem item)
        {
            if (canRename == null)
                return false;
            return canRename(item as T);
        }

        protected override bool CanBeParent(UnityTreeViewItem item)
        {
            if (canBeParent == null)
                return base.CanBeParent(item);
            return canBeParent(item as T);
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
            ItemRowGUI(args.item as T, args);
        }

        protected virtual void ItemRowGUI(T item, RowGUIArgs args)
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
                GUI.Label(labelRect, EditorGUIUtility.TrTextContent(args.label, item.icon));
            }
        }

        public T AddMenuItem(string path, char separator = '/', bool split = true)
        {
            return AddMenuItemTo(RootItem as T, path, null, separator, split);
        }

        public T AddMenuItem(string path, Texture2D icon, char separator = '/', bool split = true)
        {
            return AddMenuItemTo(RootItem as T, path, icon, separator, split);
        }

        public T GetOrAddMenuItem(string path, Texture2D icon = null, char separator = '/', bool split = true)
        {
            return GetOrAddMenuItemTo(root, path, icon, separator, split);
        }

        public T AddMenuItemTo(T parent, string path, Texture2D icon = null, char separator = '/', bool split = true)
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
                        parent.AddChild(tmpParent);
                        itemMap[tmpParent.id] = (T)tmpParent;
                    }

                    parent = (T)tmpParent;
                }
            }

            var item = itemPool.Spawn();
            item.icon = icon;
            item.id = GenerateID();
            item.displayName = name;
            parent.AddChild(item);
            itemMap[item.id] = item;
            return item;
        }

        public T GetOrAddMenuItemTo(T parent, string path, Texture2D icon = null, char separator = '/', bool split = true)
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
                        parent.AddChild(tmpParent);
                        itemMap[tmpParent.id] = (T)tmpParent;
                    }

                    parent = (T)tmpParent;
                }
            }

            if (!parent.ChildrenMap.TryGetValue(name, out var child))
            {
                child = itemPool.Spawn();
                child.icon = icon;
                child.id = GenerateID();
                child.displayName = name;
                parent.AddChild(child);
                itemMap[child.id] = (T)child;
            }

            return (T)child;
        }

        public void Remove(T treeViewItem)
        {
            if (treeViewItem == null || treeViewItem.parent == null)
                return;

            treeViewItem.parent.RemoveChild(treeViewItem);
        }

        public T FindItem(int id)
        {
            if (itemMap.TryGetValue(id, out var item))
            {
                return item;
            }

            return FindItem(id, rootItem) as T;
        }

        public T FindItem(string path, char separator = '/', bool split = true)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            var parent = (T)RootItem;
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

                    parent = (T)result;
                }
            }
            else
            {
                parent.ChildrenMap.TryGetValue(path, out result);
            }

            return (T)result;
        }

        public IEnumerable<T> Items()
        {
            return Foreach(RootItem as T);

            IEnumerable<T> Foreach(T parent)
            {
                foreach (var item in parent.children)
                {
                    yield return item as T;
                    foreach (var child in Foreach(item as T))
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
            if (Root.hasChildren)
            {
                Root.Clear();
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
            if (root.hasChildren)
            {
                Root.Clear();
            }

            lastItemId = 0;
        }

        private int lastItemId = 0;

        private int GenerateID()
        {
            return lastItemId++;
        }

        public class TreeViewItemPool : BaseObjectPool<T>
        {
            protected override T Create()
            {
                return new T();
            }

            protected override void OnRecycle(T unit)
            {
                unit.Reset();
                base.OnRecycle(unit);
            }
        }
    }


    public class TreeView : TreeView<TreeViewItem>
    {
        public TreeView(TreeViewState state) : base(state)
        {
        }

        public TreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {
        }

        public TreeView(TreeViewState state, TreeViewItemPool itemPool) : base(state, itemPool)
        {
        }

        public TreeView(TreeViewState state, MultiColumnHeader multiColumnHeader, TreeViewItemPool itemPool) : base(state, multiColumnHeader, itemPool)
        {
        }
    }
}
#endif