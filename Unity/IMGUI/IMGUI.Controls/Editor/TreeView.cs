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
using Jiange;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityTreeView = UnityEditor.IMGUI.Controls.TreeView;
using UnityTreeViewItem = UnityEditor.IMGUI.Controls.TreeViewItem;

namespace JiangeEditor.IMGUI.Controls
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
         int NextId(object userData);

         void Reset();
    }

    public class IdGenerator : IIdGenerator
    {
        private int startId;
        private int lastItemId;

        public IdGenerator()
        {
            this.lastItemId = 0;
            this.startId = 0;
        }

        public IdGenerator(int startId)
        {
            this.lastItemId = startId;
            this.startId = startId;
        }

        public int NextId(object userData)
        {
            return lastItemId++;
        }

        public void Reset()
        {
            this.lastItemId = startId;
        }
    }

    public abstract class TreeView<T> : UnityTreeView where T : TreeViewItem, new()
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

        public T Root
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

        public int Count => itemMap.Count;

        public abstract IIdGenerator IdGenerator { get; }

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
            return Root;
        }

        protected sealed override bool DoesItemMatchSearch(UnityTreeViewItem item, string search)
        {
            return DoesItemMatchSearch(item as T, search);
        }

        protected virtual bool DoesItemMatchSearch(T item, string search)
        {
            return base.DoesItemMatchSearch(item, search);
        }

        protected sealed override bool CanMultiSelect(UnityTreeViewItem item)
        {
            return CanMultiSelect((T)item);
        }

        protected virtual bool CanMultiSelect(T item)
        {
            if (canMultiSelect == null)
                return false;
            return canMultiSelect(item);
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
            var indentRect = args.rowRect;
            if (hasSearch)
                indentRect.xMin += depthIndentWidth;
            else
                indentRect.xMin += args.item.depth * depthIndentWidth + depthIndentWidth;

            ItemRowGUI(args.item as T, args, indentRect);
        }

        protected virtual void ItemRowGUI(T item, RowGUIArgs args, Rect indentRect)
        {
            DefaultRowGUI(args, indentRect);
        }

        protected void DefaultRowGUI(RowGUIArgs args, Rect indexRect)
        {
            if (!args.isRenaming)
            {
                GUI.Label(indexRect, EditorGUIUtility.TrTextContent(args.label, args.item.icon));
            }
        }

        public T AddMenuItem(string path, char separator = '/', bool split = true, object userData = null)
        {
            return AddMenuItemTo(Root, path, null, separator, split);
        }

        public T AddMenuItem(string path, Texture2D icon, char separator = '/', bool split = true, object userData = null)
        {
            return AddMenuItemTo(Root, path, icon, separator, split);
        }

        public T GetOrAddMenuItem(string path, Texture2D icon = null, char separator = '/', bool split = true, object userData = null)
        {
            return GetOrAddMenuItemTo(Root, path, icon, separator, split);
        }

        public T AddMenuItemTo(T parent, string path, Texture2D icon = null, char separator = '/', bool split = true, object userData = null)
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
                        tmpParent.id = IdGenerator.NextId(userData);
                        tmpParent.displayName = p[i];
                        parent.AddChild(tmpParent);
                        itemMap[tmpParent.id] = (T)tmpParent;
                    }

                    parent = (T)tmpParent;
                }
            }

            var item = itemPool.Spawn();
            item.icon = icon;
            item.id = IdGenerator.NextId(userData);
            item.displayName = name;
            item.userData = userData;
            parent.AddChild(item);
            itemMap[item.id] = item;
            return item;
        }

        public T GetOrAddMenuItemTo(T parent, string path, Texture2D icon = null, char separator = '/', bool split = true, object userData = null)
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
                        tmpParent.id = IdGenerator.NextId(userData);
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
                child.id = IdGenerator.NextId(userData);
                child.displayName = name;
                child.userData = userData;
                parent.AddChild(child);
                itemMap[child.id] = (T)child;
            }

            return (T)child;
        }

        public T InsertMenuItem(string path, int index, char separator = '/', bool split = true, object userData = null)
        {
            return InsertMenuItemTo(Root, path, index, null, separator, split);
        }

        public T InsertMenuItem(string path, int index, Texture2D icon, char separator = '/', bool split = true, object userData = null)
        {
            return InsertMenuItemTo(Root, path, index, icon, separator, split);
        }

        public T GetOrInsertMenuItem(string path, int index, Texture2D icon = null, char separator = '/', bool split = true, object userData = null)
        {
            return GetOrInsertMenuItemTo(root, path, index, icon, separator, split);
        }

        public T InsertMenuItemTo(T parent, string path, int index, Texture2D icon = null, char separator = '/', bool split = true, object userData = null)
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
                        tmpParent.id = IdGenerator.NextId(userData);
                        tmpParent.displayName = p[i];
                        parent.AddChild(tmpParent);
                        itemMap[tmpParent.id] = (T)tmpParent;
                    }

                    parent = (T)tmpParent;
                }
            }

            var item = itemPool.Spawn();
            item.icon = icon;
            item.id = IdGenerator.NextId(userData);
            item.displayName = name;
            item.userData = userData;
            parent.InsertChild(index, item);
            itemMap[item.id] = item;
            return item;
        }

        public T GetOrInsertMenuItemTo(T parent, string path, int index, Texture2D icon = null, char separator = '/', bool split = true, object userData = null)
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
                        tmpParent.id = IdGenerator.NextId(userData);
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
                child.id = IdGenerator.NextId(userData);
                child.displayName = name;
                child.userData = userData;
                parent.InsertChild(index, child);
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
            return Foreach(Root);

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
        public override IIdGenerator IdGenerator { get; } = new IdGenerator(20000);
        
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