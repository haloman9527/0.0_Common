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
using Moyo;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityTreeView = UnityEditor.IMGUI.Controls.TreeView;
using UnityTreeViewItem = UnityEditor.IMGUI.Controls.TreeViewItem;

namespace Moyo.UnityEditors.IMGUI.Controls
{
    public class TreeViewItem : UnityTreeViewItem
    {
        public object userData;
        private Dictionary<string, TreeViewItem> m_childrenMap;

        public new TreeViewItem parent
        {
            get => base.parent as TreeViewItem;
            set => base.parent = value;
        }

        public new List<UnityTreeViewItem> children
        {
            get
            {
                if (base.children == null)
                    base.children = new List<UnityTreeViewItem>();
                return base.children;
            }
            set => base.children = value;
        }

        public IReadOnlyDictionary<string, TreeViewItem> ChildrenMap
        {
            get
            {
                CheckChildrenMap();
                return m_childrenMap;
            }
        }

        private void CheckChildrenMap()
        {
            if (m_childrenMap == null)
            {
                m_childrenMap = new Dictionary<string, TreeViewItem>();
                if (hasChildren)
                {
                    foreach (var child in children)
                    {
                        m_childrenMap[child.displayName] = (TreeViewItem)child;
                    }
                }
            }
        }

        public void Clear()
        {
            this.userData = null;

            if (this.hasChildren)
            {
                this.children.Clear();
            }

            if (this.m_childrenMap != null)
            {
                this.m_childrenMap.Clear();
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

            if (this.m_childrenMap != null)
            {
                this.m_childrenMap.Clear();
            }
        }

        public new void AddChild(UnityTreeViewItem item)
        {
            throw new Exception("禁用此行为");
        }

        public void AddChild(TreeViewItem item)
        {
            CheckChildrenMap();
            this.children.Add(item);
            this.m_childrenMap[item.displayName] = item;
            item.parent = this;
            item.depth = this.depth + 1;
        }

        public void InsertChild(int index, TreeViewItem item)
        {
            CheckChildrenMap();
            this.children.Insert(index, item);
            this.m_childrenMap[item.displayName] = item;
            item.parent = this;
            item.depth = this.depth + 1;
        }

        public void RemoveChild(TreeViewItem item)
        {
            if (!children.Remove(item))
            {
                return;
            }

            if (m_childrenMap != null && m_childrenMap.TryGetValue(item.displayName, out var child) && child == item)
            {
                m_childrenMap.Remove(item.displayName);
            }

            item.parent = null;
        }

        public bool IsChildOf(TreeViewItem item)
        {
            var tmp = this;
            while (tmp != null)
            {
                if (tmp.parent == item)
                    return true;

                tmp = tmp.parent;
            }

            return false;
        }
    }

    public interface IIdGenerator
    {
        int NextId();

        void Reset();
    }

    public class IdGenerator : IIdGenerator
    {
        private int lastItemId;

        public int NextId()
        {
            return lastItemId++;
        }

        public void Reset()
        {
            this.lastItemId = 0;
        }
    }

    public abstract class TreeView : UnityTreeView
    {
        private TreeViewItem root;
        private TreeViewItemPool itemPool;
        private bool sharedItemPool;
        private Dictionary<int, TreeViewItem> itemMap = new Dictionary<int, TreeViewItem>();

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

        public TreeViewItem Root
        {
            get
            {
                if (root == null)
                {
                    root = new TreeViewItem() { id = -1, depth = -1, displayName = "Root" };
                }

                if (!root.hasChildren)
                {
                    root.children = new List<UnityTreeViewItem>();
                }

                return root;
            }
        }

        public int Count => itemMap.Count;

        public virtual IIdGenerator IdGenerator { get; } = new IdGenerator();

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
            SortRecursive(Root);

            void SortRecursive(TreeViewItem item)
            {
                if (!item.hasChildren)
                    return;

                item.children.QuickSort(comparer);
                foreach (var child in item.children)
                {
                    SortRecursive((TreeViewItem)child);
                }
            }
        }

        protected override UnityTreeViewItem BuildRoot() => this.BuildRootItem();

        protected virtual TreeViewItem BuildRootItem() => Root;

        protected sealed override IList<UnityTreeViewItem> BuildRows(UnityTreeViewItem root) => base.BuildRows(root);

        public sealed override IList<UnityTreeViewItem> GetRows() => base.GetRows();

        protected sealed override bool DoesItemMatchSearch(UnityTreeViewItem item, string search) => DoesItemMatchSearch(item as TreeViewItem, search);

        protected virtual bool DoesItemMatchSearch(TreeViewItem item, string search) => base.DoesItemMatchSearch(item, search);

        protected sealed override bool CanMultiSelect(UnityTreeViewItem item) => CanMultiSelect((TreeViewItem)item);

        protected virtual bool CanMultiSelect(TreeViewItem item) => false;

        protected sealed override bool CanRename(UnityTreeViewItem item) => ItemCanRename((TreeViewItem)item);

        protected virtual bool ItemCanRename(TreeViewItem item) => false;

        protected sealed override bool CanBeParent(UnityTreeViewItem item) => ItemCanBeParent((TreeViewItem)item);

        protected virtual bool ItemCanBeParent(TreeViewItem item) => true;

        protected sealed override void RenameEnded(RenameEndedArgs args)
        {
            if (args.acceptedRename)
            {
                this.ItemRenameEnded(FindItem(args.itemID), args.originalName, args.newName);
            }
        }

        protected virtual void ItemRenameEnded(TreeViewItem item, string oldName, string newName)
        {
        }

        protected sealed override void SingleClickedItem(int id)
        {
            var item = FindItem(id);
            if (item != null)
                ItemSingleClicked(item);
        }

        protected virtual void ItemSingleClicked(TreeViewItem item)
        {
        }

        protected sealed override void DoubleClickedItem(int id)
        {
            var item = FindItem(id);
            if (item != null)
                ItemDoubleClicked(item);
        }

        protected virtual void ItemDoubleClicked(TreeViewItem item)
        {
        }

        protected sealed override void ContextClickedItem(int id)
        {
            var item = FindItem(id);
            if (item != null)
                ItemContextClicked(item);
        }

        protected virtual void ItemContextClicked(TreeViewItem item)
        {
        }

        protected sealed override void RowGUI(RowGUIArgs args)
        {
            var indentRect = args.rowRect;
            if (hasSearch)
                indentRect.xMin += depthIndentWidth;
            else
                indentRect.xMin += args.item.depth * depthIndentWidth + depthIndentWidth;

            ItemRowGUI(args.item as TreeViewItem, args, indentRect);
        }

        protected virtual void ItemRowGUI(TreeViewItem item, RowGUIArgs args, Rect indentRect)
        {
            if (!args.isRenaming)
            {
                GUI.Label(indentRect, EditorGUIUtility.TrTextContent(args.label, args.item.icon));
            }
        }

        public TreeViewItem AddMenuItem(string path, char separator = '/', bool split = true)
        {
            return AddMenuItemTo(Root, path, null, separator, split);
        }

        public TreeViewItem AddMenuItem(string path, Texture2D icon, char separator = '/', bool split = true)
        {
            return AddMenuItemTo(Root, path, icon, separator, split);
        }

        public TreeViewItem GetOrAddMenuItem(string path, Texture2D icon = null, char separator = '/', bool split = true)
        {
            return GetOrAddMenuItemTo(Root, path, icon, separator, split);
        }

        public TreeViewItem AddMenuItemTo(TreeViewItem parent, string path, Texture2D icon = null, char separator = '/', bool split = true)
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
                        tmpParent.id = IdGenerator.NextId();
                        tmpParent.displayName = p[i];
                        parent.AddChild(tmpParent);
                        itemMap[tmpParent.id] = tmpParent;
                    }

                    parent = tmpParent;
                }
            }

            var item = itemPool.Spawn();
            item.icon = icon;
            item.id = IdGenerator.NextId();
            item.displayName = name;
            parent.AddChild(item);
            itemMap[item.id] = item;
            return item;
        }

        public TreeViewItem GetOrAddMenuItemTo(TreeViewItem parent, string path, Texture2D icon = null, char separator = '/', bool split = true)
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
                        tmpParent.id = IdGenerator.NextId();
                        tmpParent.displayName = p[i];
                        parent.AddChild(tmpParent);
                        itemMap[tmpParent.id] = tmpParent;
                    }

                    parent = tmpParent;
                }
            }

            if (!parent.ChildrenMap.TryGetValue(name, out var child))
            {
                child = itemPool.Spawn();
                child.icon = icon;
                child.id = IdGenerator.NextId();
                child.displayName = name;
                parent.AddChild(child);
                itemMap[child.id] = child;
            }

            return child;
        }

        public TreeViewItem InsertMenuItem(string path, int index, char separator = '/', bool split = true)
        {
            return InsertMenuItemTo(Root, path, index, null, separator, split);
        }

        public TreeViewItem InsertMenuItem(string path, int index, Texture2D icon, char separator = '/', bool split = true)
        {
            return InsertMenuItemTo(Root, path, index, icon, separator, split);
        }

        public TreeViewItem GetOrInsertMenuItem(string path, int index, Texture2D icon = null, char separator = '/', bool split = true)
        {
            return GetOrInsertMenuItemTo(root, path, index, icon, separator, split);
        }

        public TreeViewItem InsertMenuItemTo(TreeViewItem parent, string path, int index, Texture2D icon = null, char separator = '/', bool split = true)
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
                        tmpParent.id = IdGenerator.NextId();
                        tmpParent.displayName = p[i];
                        parent.AddChild(tmpParent);
                        itemMap[tmpParent.id] = tmpParent;
                    }

                    parent = tmpParent;
                }
            }

            var item = itemPool.Spawn();
            item.icon = icon;
            item.id = IdGenerator.NextId();
            item.displayName = name;
            parent.InsertChild(index, item);
            itemMap[item.id] = item;
            return item;
        }

        public TreeViewItem GetOrInsertMenuItemTo(TreeViewItem parent, string path, int index, Texture2D icon = null, char separator = '/', bool split = true)
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
                        tmpParent.id = IdGenerator.NextId();
                        tmpParent.displayName = p[i];
                        parent.AddChild(tmpParent);
                        itemMap[tmpParent.id] = tmpParent;
                    }

                    parent = tmpParent;
                }
            }

            if (!parent.ChildrenMap.TryGetValue(name, out var child))
            {
                child = itemPool.Spawn();
                child.icon = icon;
                child.id = IdGenerator.NextId();
                child.displayName = name;
                parent.InsertChild(index, child);
                itemMap[child.id] = child;
            }

            return child;
        }

        public void Remove(TreeViewItem treeViewItem)
        {
            if (treeViewItem == null || treeViewItem.parent == null)
                return;

            treeViewItem.parent.RemoveChild(treeViewItem);
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

            var parent = Root;
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
            return Enumerate(Root);

            IEnumerable<TreeViewItem> Enumerate(TreeViewItem parent)
            {
                foreach (var item in parent.children)
                {
                    yield return (TreeViewItem)item;
                    foreach (var child in Enumerate((TreeViewItem)item))
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

            IdGenerator.Reset();
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

            IdGenerator.Reset();
        }

        public class TreeViewItemPool : ObjectPool<TreeViewItem>
        {
            protected override TreeViewItem Create()
            {
                return new TreeViewItem();
            }

            protected override void OnRecycle(TreeViewItem unit)
            {
                unit.Reset();
                base.OnRecycle(unit);
            }
        }
    }
}

public class SimpleTreeView : Moyo.UnityEditors.IMGUI.Controls.TreeView
{
    public Action<IList<int>> onSelectionChanged;
    public Action onKeyEvent;
    public Action onContextClicked;
    public Action<Moyo.UnityEditors.IMGUI.Controls.TreeViewItem> onContextClickedItem;
    public Action<Moyo.UnityEditors.IMGUI.Controls.TreeViewItem> onSingleClickedItem;
    public Action<Moyo.UnityEditors.IMGUI.Controls.TreeViewItem> onDoubleClickedItem;
    public Action<Moyo.UnityEditors.IMGUI.Controls.TreeViewItem, string, string> renameEnded;

    public Func<Moyo.UnityEditors.IMGUI.Controls.TreeViewItem, string, bool> doesItemMatchSearch;
    public Func<Moyo.UnityEditors.IMGUI.Controls.TreeViewItem, bool> canRename;
    public Func<Moyo.UnityEditors.IMGUI.Controls.TreeViewItem, bool> canMultiSelect;
    public Func<Moyo.UnityEditors.IMGUI.Controls.TreeViewItem, bool> canBeParent;

    public SimpleTreeView(TreeViewState state) : base(state)
    {
    }

    public SimpleTreeView(TreeViewState state, TreeViewItemPool itemPool) : base(state, itemPool)
    {
    }

    protected override void SelectionChanged(IList<int> selectedIds)
    {
        onSelectionChanged?.Invoke(selectedIds);
    }

    protected override void KeyEvent()
    {
        onKeyEvent?.Invoke();
    }

    protected override void ContextClicked()
    {
        onContextClicked?.Invoke();
    }

    protected override void ItemContextClicked(Moyo.UnityEditors.IMGUI.Controls.TreeViewItem item)
    {
        onContextClickedItem?.Invoke(item);
    }

    protected override void ItemSingleClicked(Moyo.UnityEditors.IMGUI.Controls.TreeViewItem item)
    {
        onSingleClickedItem?.Invoke(item);
    }

    protected override void ItemDoubleClicked(Moyo.UnityEditors.IMGUI.Controls.TreeViewItem item)
    {
        onDoubleClickedItem?.Invoke(item);
    }

    protected override void ItemRenameEnded(Moyo.UnityEditors.IMGUI.Controls.TreeViewItem item, string oldName, string newName)
    {
        renameEnded?.Invoke(item, oldName, newName);
    }

    protected override bool DoesItemMatchSearch(Moyo.UnityEditors.IMGUI.Controls.TreeViewItem item, string search)
    {
        return base.DoesItemMatchSearch(item, search) || (doesItemMatchSearch != null && doesItemMatchSearch.Invoke(item, search));
    }

    protected override bool ItemCanRename(Moyo.UnityEditors.IMGUI.Controls.TreeViewItem item)
    {
        return canRename == null ? base.ItemCanRename(item) : canRename(item);
    }

    protected override bool CanMultiSelect(Moyo.UnityEditors.IMGUI.Controls.TreeViewItem item)
    {
        return canMultiSelect == null ? base.CanMultiSelect(item) : canMultiSelect(item);
    }

    protected override bool ItemCanBeParent(Moyo.UnityEditors.IMGUI.Controls.TreeViewItem item)
    {
        return canBeParent == null ? base.ItemCanBeParent(item) : canBeParent(item);
    }
}
#endif