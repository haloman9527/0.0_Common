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
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace CZToolKit.Core.Editors
{
    public class CZAdvancedDropDownItem : AdvancedDropdownItem
    {
        public event Action onSelected;

        public CZAdvancedDropDownItem(string name) : base(name) { }

        public void Selected()
        {
            onSelected?.Invoke();
        }
    }

    public class CZAdvancedDropDown : AdvancedDropdown
    {
        const string DEFAULT_ROOT_NAME = "Root";
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

        string rootName;
        CZAdvancedDropDownItem root;
        public event Action<CZAdvancedDropDownItem> onItemSelected;
        public Vector2 MinimumSize { get { return minimumSize; } set { minimumSize = value; } }
        CZAdvancedDropDownItem Root
        {
            get
            {
                if (root == null) root = new CZAdvancedDropDownItem(rootName) { id = GenerateID() };
                return root;
            }
        }

        public CZAdvancedDropDown() : this(new AdvancedDropdownState())
        {
            this.rootName = DEFAULT_ROOT_NAME;
        }
        public CZAdvancedDropDown(string rootName) : this(new AdvancedDropdownState())
        {
            this.rootName = rootName;
        }
        public CZAdvancedDropDown(string rootName, AdvancedDropdownState state) : this(state)
        {
            this.rootName = rootName;
        }
        public CZAdvancedDropDown(AdvancedDropdownState state) : base(state) { }

        // 添加一个选项
        public CZAdvancedDropDownItem Add(string _path, Texture2D _icon = null)
        {
            SplitMenuPath(_path, out _path, out string name);
            AdvancedDropdownItem parent = Root;
            if (!string.IsNullOrEmpty(_path))
            {
                string[] path = _path.Split('/');
                for (int i = 0; i < path.Length; i++)
                {
                    CZAdvancedDropDownItem tempItem = parent.children.FirstOrDefault(_item => _item.name == path[i]) as CZAdvancedDropDownItem;
                    if (tempItem != null)
                        parent = tempItem;
                    else
                    {
                        tempItem = new CZAdvancedDropDownItem(path[i]) { id = GenerateID() };
                        parent.AddChild(tempItem);
                        parent = tempItem;
                    }
                }
            }
            CZAdvancedDropDownItem item = new CZAdvancedDropDownItem(name);
            item.id = GenerateID();
            item.icon = _icon;
            parent.AddChild(item);
            return item;
        }
        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            base.ItemSelected(item);
            onItemSelected?.Invoke(item as CZAdvancedDropDownItem);
            (item as CZAdvancedDropDownItem).Selected();
        }
        protected override AdvancedDropdownItem BuildRoot() { return Root; }

        public void Show(Rect _buttonRect, float maxHeight)
        {
            if (MinimumSize == Vector2.zero)
                MinimumSize = new Vector2(200, 200);

            base.Show(_buttonRect);

            var window = EditorWindow.focusedWindow;

            if (window == null)
            {
                Debug.LogWarning("EditorWindow.focusedWindow was null.");
                return;
            }

            if (!string.Equals(window.GetType().Namespace, typeof(AdvancedDropdown).Namespace))
            {
                Debug.LogWarning("EditorWindow.focusedWindow " + EditorWindow.focusedWindow.GetType().FullName + " was not in expected namespace.");
                return;
            }

            window.minSize = MinimumSize;
            window.maxSize = new Vector2(window.maxSize.x, maxHeight);

            Vector2 size = window.position.size;
            size.x = Mathf.Clamp(size.x, window.minSize.x, window.maxSize.x);
            size.y = Mathf.Clamp(size.y, window.minSize.y, window.maxSize.y);

            window.ShowAsDropDown(GUIUtility.GUIToScreenRect(_buttonRect), size);
        }

        int id;
        int GenerateID() { return id++; }
    }
}
