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
        #region GUIContent
        public class GUIContentPool
        {
            Dictionary<string, GUIContent> GUIContentsCache = new Dictionary<string, GUIContent>();

            public GUIContent TextContent(string _name)
            {
                GUIContent content;
                if (!GUIContentsCache.TryGetValue(_name, out content))
                    content = new GUIContent(_name);
                content.tooltip = string.Empty;
                content.image = null;
                return content;
            }

            public GUIContent TextContent(string _name, Texture2D _image)
            {
                GUIContent content = TextContent(_name);
                content.image = _image;
                return content;
            }

            public GUIContent TextContent(string _name, string _tooltip)
            {
                GUIContent content = TextContent(_name);
                content.tooltip = _tooltip;
                return content;
            }

            public GUIContent TextContent(string _name, string _tooltip, Texture2D _image)
            {
                GUIContent content = TextContent(_name);
                content.tooltip = _tooltip;
                content.image = _image;
                return content;
            }
        }

        static GUIContentPool ContentPool = new GUIContentPool();

        public static GUIContent TextContent(string _name)
        {
            return ContentPool.TextContent(_name);
        }

        public static GUIContent TextContent(string _name, Texture2D _image)
        {
            return ContentPool.TextContent(_name, _image);
        }

        public static GUIContent TextContent(string _name, string _tooltip)
        {
            return ContentPool.TextContent(_name, _tooltip);
        }

        public static GUIContent TextContent(string _name, string _tooltip, Texture2D _image)
        {
            return ContentPool.TextContent(_name, _tooltip, _image);
        }
        #endregion

        #region ContextData
        public interface IContextData { }

        public sealed class ContextData<T> : IContextData { public T value; }

        public class ContextDataCache
        {
            Dictionary<string, IContextData> ContextDatas = new Dictionary<string, IContextData>();

            public bool TryGetContextData<T>(string _key, out ContextData<T> _contextData)
            {
                if (ContextDatas.TryGetValue(_key, out IContextData _data))
                {
                    if (_data is ContextData<T> _t_data)
                    {
                        _contextData = _t_data;
                        return true;
                    }
                }
                _contextData = new ContextData<T>();
                ContextDatas[_key] = _contextData;
                return false;
            }

            public ContextData<T> GetContextData<T>(string _key, T _default = default)
            {
                if (ContextDatas.TryGetValue(_key, out IContextData _data))
                {
                    if (_data is ContextData<T> _t_data)
                    {
                        return _t_data;
                    }
                }
                var contextData = new ContextData<T>();
                ContextDatas[_key] = contextData;
                return contextData;
            }
        }

        static ContextDataCache ContextDatas = new ContextDataCache();

        public static bool TryGetContextData<T>(string _key, out ContextData<T> _contextData)
        {
            return ContextDatas.TryGetContextData(_key, out _contextData);
        }

        public static ContextData<T> GetContextData<T>(string _key, T _default = default)
        {
            return ContextDatas.GetContextData(_key, _default);
        }
        #endregion
    }
}