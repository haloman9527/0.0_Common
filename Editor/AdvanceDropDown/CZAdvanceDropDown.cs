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
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace CZToolKit.Core.Editors
{
    public class CZAdvancedDropDown : AdvancedDropdown
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

        AdvancedDropdownItem root;
        Action<AdvancedDropdownItem> onItemSelected;

        AdvancedDropdownItem Root
        {
            get
            {
                if (root == null) root = new AdvancedDropdownItem("Root") { id = GenerateID() };
                return root;
            }
        }

        public CZAdvancedDropDown() : this(new AdvancedDropdownState()) { }

        public CZAdvancedDropDown(AdvancedDropdownState state) : base(state) { }

        protected override AdvancedDropdownItem BuildRoot() { return Root; }

        public void Add(string _path, Texture2D _icon = null)
        {
            SplitMenuPath(_path, out _path, out string name);
            AdvancedDropdownItem parent = Root;
            if (!string.IsNullOrEmpty(_path))
            {
                string[] path = _path.Split('/');
                for (int i = 0; i < path.Length; i++)
                {
                    AdvancedDropdownItem tempItem = parent.children.FirstOrDefault(_item => _item.name == path[i]);
                    if (tempItem != null)
                        parent = tempItem;
                    else
                    {
                        tempItem = new AdvancedDropdownItem(path[i]) { id = GenerateID() };
                        parent.AddChild(tempItem);
                        parent = tempItem;
                    }
                }
            }
            AdvancedDropdownItem item = new AdvancedDropdownItem(name);
            item.id = GenerateID();
            item.icon = _icon;
            parent.AddChild(item);
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            base.ItemSelected(item);
            onItemSelected?.Invoke(item);
        }

        int id;
        int GenerateID()
        {
            return id++;
        }
    }
}
