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
        public Action onContextClicked;
        public Action onDoubleClicked;
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

        public event Action<IList<int>> onSelectionChanged;
        public event Action onContextClicked;
        public event Action<CZTreeViewItem> onContextClickedItem;
        public event Action<CZTreeViewItem> onSingleClickedItem;
        public event Action<CZTreeViewItem> onDoubleClickedItem;

        public float RowHeight { get => rowHeight; set => rowHeight = value; }
        public bool ShowBoder { get => showBorder; set => showBorder = value; }
        public bool ShowAlternatingRowBackgrounds { get => showAlternatingRowBackgrounds; set => showAlternatingRowBackgrounds = value; }
        public IReadOnlyList<TreeViewItem> Items { get => items; }

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
            if (path.Length > 1)
            {
                for (int i = 0; i < path.Length - 1; i++)
                {
                    CZTreeViewItem currentParent = current.Find(t => t.displayName == path[i]) as CZTreeViewItem;
                    if (currentParent == null)
                    {
                        currentParent = new CZTreeViewItem();
                        currentParent.children = new List<TreeViewItem>();
                        currentParent.displayName = path[i];
                        currentParent.id = itemCount;
                        current.Add(currentParent);
                        itemCount++;
                    }
                    current = currentParent.children;
                }
            }

            _treeViewItem.id = itemCount;
            _treeViewItem.displayName = path[path.Length - 1];
            _treeViewItem.children = new List<TreeViewItem>();
            current.Add(_treeViewItem);
            itemCount++;
        }

        public void Remove(CZTreeViewItem _treeViewItem)
        {
            items.Remove(_treeViewItem);
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

        public void Sort(Func<TreeViewItem, TreeViewItem, bool> _func)
        {
            SortChildren(items, _func);
        }

        void SortChildren(List<TreeViewItem> _items, Func<TreeViewItem, TreeViewItem, bool> _func)
        {
            QuickSort(_items, _func);
            foreach (var item in _items)
            {
                if (item.hasChildren)
                    SortChildren(item.children, _func);
            }
        }

        /// <summary> 快速排序(第二个参数是中间值) </summary>
        void QuickSort<T>(List<T> _original, Func<T, T, bool> _func)
        {
            if (_original.Count == 0)
                return;
            if (_original.Count == 1)
                return;

            // 抽取一个数据作为中间值
            int index = UnityEngine.Random.Range(0, _original.Count);
            T rN = _original[index];

            // 声明小于中间值的列表
            List<T> left = new List<T>(Math.Max(4, _original.Count / 2));
            // 声明大于中间值的列表
            List<T> right = new List<T>(Math.Max(4, _original.Count / 2));
            // 遍历数组，与中间值比较，小于中间值的放在left，大于中间值的放在right
            for (int i = 0; i < _original.Count; i++)
            {
                if (i == index) continue;

                if (_func(_original[i], rN))
                    left.Add(_original[i]);
                else
                    right.Add(_original[i]);
            }

            _original.Clear();

            // 如果左列表元素个数不为0，就把左列表也排序
            if (left.Count != 0)
            {
                QuickSort(left, _func);
                _original.AddRange(left);
            }
            _original.Add(rN);
            // 如果右列表元素个数不为0，就把右列表也排序
            if (right.Count != 0)
            {
                QuickSort(right, _func);
                _original.AddRange(right);
            }
            return;
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
            items.Clear();
        }
    }
}
