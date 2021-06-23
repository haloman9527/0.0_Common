#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 
 *
 */
#endregion
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
    }

    public abstract class CZTreeView : TreeView
    {
        private static void SplitMenuPath(string _menuPath, out string _path, out string _name)
        {
            _menuPath = _menuPath.Trim('/');
            int num = _menuPath.LastIndexOf('/');
            if (num == -1)
            {
                _path = "";
                _name = _menuPath;
                return;
            }
            _path = _menuPath.Substring(0, num);
            _name = _menuPath.Substring(num + 1);

        }

        int itemCount = 0;

        List<TreeViewItem> items = new List<TreeViewItem>();
        Dictionary<int, CZTreeViewItem> treeViewItemIDMap = new Dictionary<int, CZTreeViewItem>();

        public event Action<IEnumerable<CZTreeViewItem>> onSelectionChanged;
        public event Action onContextClicked;
        public event Action<CZTreeViewItem> onContextClickedItem;
        public event Action<CZTreeViewItem> onSingleClickedItem;
        public event Action<CZTreeViewItem> onDoubleClickedItem;

        public float RowHeight { get => rowHeight; set => rowHeight = value; }
        public bool ShowBoder { get => showBorder; set => showBorder = value; }
        public bool ShowAlternatingRowBackgrounds { get => showAlternatingRowBackgrounds; set => showAlternatingRowBackgrounds = value; }
        public IReadOnlyList<TreeViewItem> Items { get => items; }
        public IReadOnlyDictionary<int, CZTreeViewItem> TreeViewIDMap { get => treeViewItemIDMap; }

        public CZTreeView(TreeViewState state) : base(state) { }

        public CZTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader) { }

        protected override TreeViewItem BuildRoot()
        {
            TreeViewItem root = new TreeViewItem(-1, -1, "Root");
            root.children = items;
            SetupDepthsFromParentsAndChildren(root);
            return root;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            Rect rowRect = args.rowRect;
            rowRect.y += rowRect.height;
            rowRect.height = 1;
            EditorGUI.DrawRect(rowRect, new Color(0.5f, 0.5f, 0.5f, 1));

            CZTreeViewItem item = args.item as CZTreeViewItem;

            Rect labelRect = args.rowRect;
            if (hasSearch)
            {
                labelRect.x += depthIndentWidth;
                labelRect.width -= labelRect.x;
            }
            else
            {
                labelRect.x += item.depth * depthIndentWidth + depthIndentWidth;
                labelRect.width -= labelRect.x;
            }
            GUI.Label(labelRect, EditorGUIExtension.GetGUIContent(item.displayName, item.icon), EditorStylesExtension.LeftLabelStyle);
        }

        public void AddMenuItem<T>(string _path, T _treeViewItem) where T : CZTreeViewItem
        {
            if (string.IsNullOrEmpty(_path)) return;
            List<TreeViewItem> current = items;
            string[] path = _path.Split('/');
            SplitMenuPath(_path, out string par, out string name);
            if (path.Length > 1)
            {
                for (int i = 0; i < path.Length - 1; i++)
                {
                    CZMenuTreeViewItem currentParent = current.Find(t => t.displayName == path[i]) as CZMenuTreeViewItem;
                    if (currentParent == null)
                    {
                        currentParent = new CZMenuTreeViewItem();
                        currentParent.children = new List<TreeViewItem>();
                        currentParent.displayName = path[i];
                        currentParent.id = itemCount;
                        current.Add(currentParent);
                        treeViewItemIDMap[itemCount] = currentParent;
                        itemCount++;
                    }
                    current = currentParent.children;
                }
            }

            _treeViewItem.id = itemCount;
            _treeViewItem.displayName = path[path.Length - 1];
            _treeViewItem.children = new List<TreeViewItem>();
            current.Add(_treeViewItem);
            treeViewItemIDMap[itemCount] = _treeViewItem;
            itemCount++;
        }

        public void Remove(CZTreeViewItem _treeViewItem)
        {
            items.Remove(_treeViewItem);
            treeViewItemIDMap.Remove(_treeViewItem.id);
        }

        public CZTreeViewItem Find(int _id)
        {
            CZTreeViewItem item;
            TreeViewIDMap.TryGetValue(_id, out item);
            return item;
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);
            onSelectionChanged?.Invoke(FindItems(selectedIds));

            IEnumerable<CZTreeViewItem> FindItems(IList<int> _ids)
            {
                foreach (var id in _ids)
                {
                    yield return Find(id);
                }
            }
        }

        protected override void SingleClickedItem(int id)
        {
            base.SingleClickedItem(id);
            onSingleClickedItem?.Invoke(Find(id));
        }

        protected override void DoubleClickedItem(int id)
        {
            base.DoubleClickedItem(id);
            onDoubleClickedItem?.Invoke(Find(id));
        }

        protected override void ContextClicked()
        {
            base.ContextClicked();
            onContextClicked?.Invoke();
        }

        protected override void ContextClickedItem(int id)
        {
            base.ContextClickedItem(id);
            onContextClickedItem?.Invoke(Find(id));
        }

        public void Clear()
        {
            items.Clear();
            treeViewItemIDMap.Clear();
        }
    }
}
