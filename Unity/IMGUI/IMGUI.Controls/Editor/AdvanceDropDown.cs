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
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

using UnityAdvancedDropdown = UnityEditor.IMGUI.Controls.AdvancedDropdown;
using UnityAdvancedDropDownItem = UnityEditor.IMGUI.Controls.AdvancedDropdownItem;

namespace MoyoEditor.IMGUI.Controls
{
    public class AdvancedDropdownItem : UnityAdvancedDropDownItem
    {
        public object userData;

        private Dictionary<string, AdvancedDropdownItem> childrenMap;

        public Dictionary<string, AdvancedDropdownItem> ChildrenMap
        {
            get
            {
                if (childrenMap == null)
                {
                    childrenMap = new Dictionary<string, AdvancedDropdownItem>();
                }

                return childrenMap;
            }
        }
        
        public AdvancedDropdownItem(string name) : base(name)
        {
            
        }
    }

    public class AdvancedDropdown : UnityAdvancedDropdown
    {
        const string DEFAULT_ROOT_NAME = "Root";

        private string rootName;
        private AdvancedDropdownItem root;
        
        public event Action<AdvancedDropdownItem> onItemSelected;

        public Vector2 MinimumSize
        {
            get { return minimumSize; }
            set { minimumSize = value; }
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
        public AdvancedDropdownItem Add(string path, Texture2D icon = null, char seperator = '/', bool split = true)
        {
            var parent = Root;
            var name = path;
            
            if (split && path.IndexOf(seperator) != -1)
            {
                var p = path.Split(seperator);
                if (p.Length > 1)
                {
                    name = p[^1];
                    for (int i = 0; i < p.Length - 1; i++)
                    {
                        if (!parent.ChildrenMap.TryGetValue(p[i], out var child))
                        {
                            child = new AdvancedDropdownItem(p[i]) { id = GenerateID() };
                            parent.AddChild(child);
                            parent.ChildrenMap[p[i]] = child;
                        }

                        parent = child;
                    }
                }
            }

            var item = new AdvancedDropdownItem(name);
            item.id = GenerateID();
            item.icon = icon;
            parent.AddChild(item);
            parent.ChildrenMap[name] = item;
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

        private int lastId;

        private int GenerateID()
        {
            return lastId++;
        }
    }
}
#endif