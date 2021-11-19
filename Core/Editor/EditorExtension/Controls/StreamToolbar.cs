#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *      自动换行的Toolbar
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
using UnityEditor;
using UnityEngine;

namespace CZToolKit.Core.Editors
{
    [Serializable]
    public class SteamToolbar
    {
        public int selected;
        public int row = 5;
        public float lineHeight = 50;
        public GUIStyle style;
        public string[] tabs;

        public SteamToolbar(string[] tabs)
        {
            this.tabs = tabs;
        }

        public void DoLayout()
        {
            if (style == null)
                style = new GUIStyle("AppToolbarButtonMid");
            selected = Mathf.Min(tabs.Length - 1, selected);
            EditorGUILayout.BeginVertical();
            bool h = false;
            for (int i = 0; i < tabs.Length; i++)
            {
                if (row != 0 && i % row == 0)
                {
                    GUILayout.BeginHorizontal();
                    h = !h;
                }

                if (GUILayout.Toggle(selected == i ? true : false, tabs[i], style, GUILayout.Height(lineHeight)))
                    selected = i;

                if (row != 0 && i % row == row - 1)
                {
                    GUILayout.EndHorizontal();
                    h = !h;
                }
            }
            if (row != 0 && h)
                GUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
    }
}
#endif