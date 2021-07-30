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
using System.Collections.Generic;
using UnityEngine;

namespace CZToolKit.Core.Editors
{
    public static class GUIHelper
    {
        static Dictionary<string, GUIContent> GUIContentsCache = new Dictionary<string, GUIContent>();
        public static GUIContent GetGUIContent(string _name)
        {
            GUIContent content;
            if (!GUIContentsCache.TryGetValue(_name, out content))
                content = new GUIContent(_name);
            content.tooltip = string.Empty;
            content.image = null;
            return content;
        }
        public static GUIContent GetGUIContent(string _name, Texture2D _image)
        {
            GUIContent content = GetGUIContent(_name);
            content.image = _image;
            return content;
        }

        public static GUIContent GetGUIContent(string _name, string _tooltip)
        {
            GUIContent content = GetGUIContent(_name);
            content.tooltip = _tooltip;
            return content;
        }

        public static GUIContent GetGUIContent(string _name, string _tooltip, Texture2D _image)
        {
            GUIContent content = GetGUIContent(_name);
            content.tooltip = _tooltip;
            content.image = _image;
            return content;
        }

        static Dictionary<string, bool> FoldoutCache = new Dictionary<string, bool>();
        public static bool GetFoldoutBool(string _key, bool _fallback = false)
        {
            bool result;
            if (!FoldoutCache.TryGetValue(_key, out result))
                result = _fallback;
            return result;
        }
        public static void SetFoldoutBool(string _key, bool _value)
        {
            FoldoutCache[_key] = _value;
        }
    }
}