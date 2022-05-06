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
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace CZToolKit.Core.Editors
{
    public class CZAdvancedDropDownItem : AdvancedDropdownItem
    {
        public object userData;
        public event Action<CZAdvancedDropDownItem> onSelected;

        public CZAdvancedDropDownItem(string name) : base(name) { }

        public void Selected()
        {
            onSelected?.Invoke(this);
        }
    }

    public class CZAdvancedDropDown : AdvancedDropdown
    {
        const string DEFAULT_ROOT_NAME = "Root";
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
        public CZAdvancedDropDownItem Add(string path, Texture2D icon = null)
        {
            SplitMenuPath(path, out path, out string name);
            AdvancedDropdownItem parent = Root;
            if (!string.IsNullOrEmpty(path))
            {
                string[] tmpPath = path.Split('/');
                for (int i = 0; i < tmpPath.Length; i++)
                {
                    CZAdvancedDropDownItem tempItem = parent.children.FirstOrDefault(_item => _item.name == tmpPath[i]) as CZAdvancedDropDownItem;
                    if (tempItem != null)
                        parent = tempItem;
                    else
                    {
                        tempItem = new CZAdvancedDropDownItem(tmpPath[i]) { id = GenerateID() };
                        parent.AddChild(tempItem);
                        parent = tempItem;
                    }
                }
            }
            CZAdvancedDropDownItem item = new CZAdvancedDropDownItem(name);
            item.id = GenerateID();
            item.icon = icon;
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

        public void Show(Rect buttonRect, float maxHeight)
        {
            if (MinimumSize == Vector2.zero)
                MinimumSize = new Vector2(200, 200);

            base.Show(buttonRect);

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

            window.ShowAsDropDown(GUIUtility.GUIToScreenRect(buttonRect), size);
        }

        int id;
        int GenerateID() { return id++; }
    }
}
#endif