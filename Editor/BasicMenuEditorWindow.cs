using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace CZToolKit.Core.Editors
{
    [System.Serializable]
    public class BasicMenuEditorWindow : BasicEditorWindow<BasicMenuEditorWindow>
    {
        [MenuItem("Tools/1")]
        public static void Open()
        {
            GetWindow<BasicMenuEditorWindow>();
        }

        [SerializeField]
        ResizableArea resizableArea;
        string searchText;
        SearchField searchField;
        CZMenuTreeView menuTreeView;
        TreeViewState treeViewState;

        void OnEnable()
        {
            if (resizableArea == null)
            {
                resizableArea = new ResizableArea(new Rect(0, 0, 150, position.height));
                resizableArea.minSize = new Vector2(50, 50);
                resizableArea.side = 10;
                resizableArea.EnableSide(UIDirections.Right);
            }
            resizableArea.SideOffset[UIDirections.Right] = resizableArea.side / 2;

            if (treeViewState == null)
                treeViewState = new TreeViewState();

            searchField = new SearchField();
            menuTreeView = BuildMenuTree(treeViewState);
            menuTreeView.Reload();
        }

        void OnGUI()
        {
            resizableArea.maxSize = position.size;
            Rect r = resizableArea.Position;
            r.height = position.height;
            resizableArea.Position = r;
            resizableArea.OnGUI();

            Rect searchFieldRect = resizableArea.Position;
            searchFieldRect.height = 20;
            searchFieldRect.y += 3;
            searchFieldRect.x += 5;
            searchFieldRect.width -=10;
            string tempSearchText = searchField.OnGUI(searchFieldRect, searchText);
            if (tempSearchText != searchText)
            {
                searchText = tempSearchText;
                menuTreeView.searchString = searchText;
            }

            Rect r2 = resizableArea.Position;
            r2.y += 20;
            r2.height -= 20;
            menuTreeView.OnGUI(r2);

            Rect sideRect = resizableArea.Position;
            sideRect.x += sideRect.width;
            sideRect.width = 1;
            EditorGUI.DrawRect(sideRect, new Color(0.5f, 0.5f, 0.5f, 1));

            Rect rightRect = new Rect(resizableArea.Position.width + 2, 0, position.width - resizableArea.Position.width - 4, position.height);
            rightRect.width = Mathf.Max(rightRect.width, 500);
            GUILayout.BeginArea(rightRect);
            OnRightGUI();
            GUILayout.EndArea();
        }

        protected virtual CZMenuTreeView BuildMenuTree(TreeViewState _treeViewState)
        {
            CZMenuTreeView menuTreeView = new CZMenuTreeView(_treeViewState);

            menuTreeView.AddMenuItem("1", EditorGUIUtility.FindTexture("GameManager Icon"));
            menuTreeView.AddMenuItem("1/2", EditorGUIUtility.FindTexture("BuildSettings.Xbox360"));
            menuTreeView.AddMenuItem("1/2/3").itemDrawer += rect =>
            {
                rect.x += rect.width - 50;
                rect.width = 50;
                GUI.Button(rect, "哈哈哈");
            };
            menuTreeView.AddMenuItem("1/2/3/4").itemDrawer += rect =>
            {
                rect.x += rect.width - 50;
                rect.width = 50;
                GUI.Button(rect, "哈哈哈");
            };
            menuTreeView.AddMenuItem("1/2/3/4/5");
            menuTreeView.AddMenuItem("1/2/3/4/5/6");
            menuTreeView.AddMenuItem("1/2/3/4/5/6/7");

            menuTreeView.AddMenuItem("2");
            menuTreeView.AddMenuItem("2/2");

            return menuTreeView;
        }

        protected virtual void OnRightGUI()
        {

        }
    }

    public class CZMenuTreeView : TreeView
    {
        List<CZMenuTreeViewItem> treeViewItems = new List<CZMenuTreeViewItem>();

        public CZMenuTreeView(TreeViewState state) : base(state)
        {
            rowHeight = 30;
        }

        protected override TreeViewItem BuildRoot()
        {
            TreeViewItem root = new TreeViewItem(-1, -1, "Root");
            root.children = new List<TreeViewItem>();

            int id = 0;
            foreach (var item in treeViewItems)
            {
                string[] path = item.Path.Split('/');
                TreeViewItem currentLayer = root;
                if (path.Length > 1)
                {
                    for (int i = 0; i < path.Length - 1; i++)
                    {
                        TreeViewItem child = currentLayer.children.Find(l => l.displayName == path[i]);
                        if (child == null)
                        {
                            child = new CZMenuTreeViewItem(id, i, path[i]);
                            child.children = new List<TreeViewItem>();
                            id++;
                            currentLayer.AddChild(child);
                        }
                        currentLayer = child;
                    }
                }
                item.depth = path.Length;
                item.id = id;
                id++;
                item.displayName = path[path.Length - 1];
                currentLayer.AddChild(item);
            }

            SetupDepthsFromParentsAndChildren(root);
            return root;
        }

        public CZMenuTreeViewItem AddMenuItem(string _path)
        {
            return AddMenuItem(_path, null);
        }

        public CZMenuTreeViewItem AddMenuItem(string _path, Texture2D _icon)
        {
            if (string.IsNullOrEmpty(_path)) return null;
            CZMenuTreeViewItem item = new CZMenuTreeViewItem(_path);
            item.icon = _icon;
            treeViewItems.Add(item);
            return item;
        }

        protected override bool CanRename(TreeViewItem item)
        {
            return false;
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return false;
        }

        protected override void RenameEnded(RenameEndedArgs args)
        {
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            Rect lineRect = args.rowRect;
            lineRect.y += lineRect.height;
            lineRect.height = 1;
            EditorGUI.DrawRect(lineRect, new Color(0.5f, 0.5f, 0.5f, 1));

            CZMenuTreeViewItem item = args.item as CZMenuTreeViewItem;

            Rect labelRect = args.rowRect;
            labelRect.x += item.depth * depthIndentWidth + depthIndentWidth;
            GUI.Label(labelRect, new GUIContent(item.displayName, item.icon));
            item.itemDrawer?.Invoke(args.rowRect);
        }
    }

    public class CZMenuTreeViewItem : TreeViewItem
    {
        string path;
        public Action<Rect> itemDrawer;

        public string Path { get { return path; } }

        public CZMenuTreeViewItem(string _path) : base()
        {
            path = _path;
        }

        public CZMenuTreeViewItem(int id, int depth, string displayName) : base(id, depth, displayName)
        {

        }
    }
}
