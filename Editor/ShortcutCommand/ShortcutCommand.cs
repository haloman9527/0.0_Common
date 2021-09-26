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
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace CZToolKit.Core.Editors
{
    public class ShortcutCommand : EditorWindow
    {
        static ShortcutCommand instance;

        public static ShortcutCommand Instance
        {
            get
            {
                if (instance == null)
                    instance = ScriptableObject.CreateInstance<ShortcutCommand>();
                return instance;
            }
        }

        [MenuItem("Tools/CZToolKit/Shortcut Command %&D")]
        static void Open()
        {
            Instance.position = new Rect(300, 300, 700, 500);
            Instance.ShowUtility();
        }

        TreeViewState state = new TreeViewState();
        CommandTreeView treeView;

        private void OnEnable()
        {
            instance = this;
            treeView = new CommandTreeView(state);
            treeView.Reload();
        }

        private void OnGUI()
        {
            treeView.OnGUI(new Rect(0, 0, position.width, position.height));
        }

        private void OnLostFocus()
        {
            Close();
        }
    }

    public class CommandTreeView : CZTreeView
    {
        public CommandTreeView(TreeViewState state) : base(state)
        {
        }

        public CommandTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {
        }
    }
}
