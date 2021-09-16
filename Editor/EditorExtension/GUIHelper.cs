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
        #region GUIContentCache
        static Dictionary<string, GUIContent> GUIContentsCache = new Dictionary<string, GUIContent>();

        public static GUIContent TextContent(string _name)
        {
            GUIContent content;
            if (!GUIContentsCache.TryGetValue(_name, out content))
                content = new GUIContent(_name);
            content.tooltip = string.Empty;
            content.image = null;
            return content;
        }

        public static GUIContent TextContent(string _name, Texture2D _image)
        {
            GUIContent content = TextContent(_name);
            content.image = _image;
            return content;
        }

        public static GUIContent TextContent(string _name, string _tooltip)
        {
            GUIContent content = TextContent(_name);
            content.tooltip = _tooltip;
            return content;
        }

        public static GUIContent TextContent(string _name, string _tooltip, Texture2D _image)
        {
            GUIContent content = TextContent(_name);
            content.tooltip = _tooltip;
            content.image = _image;
            return content;
        }
        #endregion

        #region ContextData
        public class ContextData { }
        public sealed class ContextData<T> : ContextData { public T value; }

        static Dictionary<string, ContextData> ContextDatas = new Dictionary<string, ContextData>();

        public static ContextData<T> GetContextData<T>(string _key, T _default = default)
        {
            if (ContextDatas.TryGetValue(_key, out ContextData _data))
            {
                if (_data is ContextData<T> _t_data)
                    return _t_data;
            }
            ContextData<T> t_data = new ContextData<T>() { value = _default };
            ContextDatas[_key] = t_data;
            return t_data;
        }
        #endregion
    }
}