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
        public Action<Rect, CZTreeViewItem> itemDrawer;
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

    public abstract class CZTreeView : TreeView
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
        internal TreeViewItem RootItem
        {
            get
            {
                if (root == null)
                {
                    root = new CZTreeViewItem() { id = -1, depth = -1, displayName = "Root" };
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

        protected override void RowGUI(RowGUIArgs args)
        {
            Rect rowRect = args.rowRect;
            rowRect.yMin = rowRect.yMax - 1;
            EditorGUI.DrawRect(rowRect, Color.black);
            CZTreeViewItem item = args.item as CZTreeViewItem;

            if (!args.isRenaming)
            {
                Rect labelRect = args.rowRect;
                if (hasSearch)
                    labelRect.xMin += depthIndentWidth;
                else
                    labelRect.xMin += item.depth * depthIndentWidth + depthIndentWidth;
                GUIContent textContent = GUIHelper.TextContent(item.displayName);
                textContent.image = item.icon;
                GUI.Label(labelRect, textContent, GUIStyles.leftLabelStyle);
            }
            if (item != null)
                item.itemDrawer?.Invoke(args.rowRect, item);
        }

        public void AddMenuItem<T>(string path, T treeViewItem) where T : CZTreeViewItem
        {
            if (string.IsNullOrEmpty(path)) return;

            CZTreeViewItem rootItem = RootItem as CZTreeViewItem;

            SplitMenuPath(path, out path, out string name);
            if (!string.IsNullOrEmpty(path))
            {
                string[] tmpPath = path.Split('/');
                for (int i = 0; i < tmpPath.Length; i++)
                {
                    CZTreeViewItem tempItem = rootItem.children.Find(item => item.displayName == tmpPath[i]) as CZTreeViewItem;
                    if (tempItem != null)
                    {
                        rootItem = tempItem;
                    }
                    else
                    {
                        tempItem = new CZTreeViewItem() { id = GenerateID(), displayName = tmpPath[i], parent = rootItem };
                        rootItem.children.Add(tempItem);
                        rootItem = tempItem;
                    }
                }
            }

            rootItem.id = GenerateID();
            rootItem.displayName = name;
            rootItem.parent = rootItem;

            rootItem.children.Add(rootItem);
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

        public void Remove(CZTreeViewItem _treeViewItem)
        {
            if (_treeViewItem == null || _treeViewItem.parent == null)
                return;
            _treeViewItem.parent.children.Remove(_treeViewItem);
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

        public void Sort(Func<TreeViewItem, TreeViewItem, bool> _func)
        {
            SortChildren(RootItem.children, _func);

            void SortChildren(List<TreeViewItem> _items, Func<TreeViewItem, TreeViewItem, bool> func)
            {
                QuickSort(_items, func);
                foreach (var item in _items)
                {
                    SortChildren(item.children, func);
                }
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