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
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

using UnityAdvancedDropdown = UnityEditor.IMGUI.Controls.AdvancedDropdown;
using UnityAdvancedDropDownItem = UnityEditor.IMGUI.Controls.AdvancedDropdownItem;

namespace CZToolKitEditor.IMGUI.Controls
{
    public class AdvancedDropdownItem : UnityAdvancedDropDownItem
    {
        public object userData;
        
        public AdvancedDropdownItem(string name) : base(name)
        {
        }
    }

    public class AdvancedDropdown : UnityAdvancedDropdown
    {
        const string DEFAULT_ROOT_NAME = "Root";

        private string rootName;
        private AdvancedDropdownItem root;
        private bool split = true;
        
        public event Action<AdvancedDropdownItem> onItemSelected;

        public Vector2 MinimumSize
        {
            get { return minimumSize; }
            set { minimumSize = value; }
        }
        
        public bool Split
        {
            get => split;
            set => split = value;
        }

        AdvancedDropdownItem Root
        {
            get
            {
                if (root == null) root = new AdvancedDropdownItem(rootName) { id = GenerateID() };
                return root;
            }
        }

        public AdvancedDropdown() : this(DEFAULT_ROOT_NAME, new AdvancedDropdownState())
        {
            
        }

        public AdvancedDropdown(string rootName) : this(rootName, new AdvancedDropdownState())
        {
            
        }

        public AdvancedDropdown(AdvancedDropdownState state) : this(DEFAULT_ROOT_NAME, state)
        {
        }

        public AdvancedDropdown(string rootName, AdvancedDropdownState state) : base(state)
        {
            this.rootName = rootName;
        }

        // 添加一个选项
        public AdvancedDropdownItem Add(string path, Texture2D icon = null, char sperator = '/')
        {
            var name = path;
            var parent = Root;
            if (split)
            {
                var p = path.Split(sperator);
                name = p[p.Length - 1];
                if (p.Length > 1)
                {
                    for (int i = 0; i < p.Length - 1; i++)
                    {
                        var tempItem = parent.children.FirstOrDefault(_item => _item.name == p[i]) as AdvancedDropdownItem;
                        if (tempItem != null)
                            parent = tempItem;
                        else
                        {
                            tempItem = new AdvancedDropdownItem(p[i]) { id = GenerateID() };
                            parent.AddChild(tempItem);
                            parent = tempItem;
                        }
                    }
                }
            }

            var item = new AdvancedDropdownItem(name);
            item.id = GenerateID();
            item.icon = icon;
            parent.AddChild(item);
            return item;
        }

        protected override void ItemSelected(UnityAdvancedDropDownItem item)
        {
            base.ItemSelected(item);
            onItemSelected?.Invoke(item as AdvancedDropdownItem);
        }

        protected override UnityAdvancedDropDownItem BuildRoot()
        {
            return Root;
        }

        public void Show(Rect rect, float maxHeight)
        {
            if (MinimumSize == Vector2.zero)
                MinimumSize = new Vector2(200, 200);

            base.Show(rect);

            var window = EditorWindow.focusedWindow;

            if (window == null)
            {
                Debug.LogWarning("EditorWindow.focusedWindow was null.");
                return;
            }

            if (!string.Equals(window.GetType().Namespace, typeof(UnityAdvancedDropdown).Namespace))
            {
                Debug.LogWarning("EditorWindow.focusedWindow " + EditorWindow.focusedWindow.GetType().FullName + " was not in expected namespace.");
                return;
            }

            window.minSize = MinimumSize;
            window.maxSize = new Vector2(window.maxSize.x, maxHeight);

            Vector2 size = window.position.size;
            size.x = Mathf.Clamp(size.x, window.minSize.x, window.maxSize.x);
            size.y = Mathf.Clamp(size.y, window.minSize.y, window.maxSize.y);
            
            window.ShowAsDropDown(GUIUtility.GUIToScreenRect(rect), size);
        }

        int id;

        int GenerateID()
        {
            return id++;
        }
    }
}
#endif