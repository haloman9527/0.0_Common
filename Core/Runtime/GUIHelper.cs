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

            public GUIContent TextContent(string name)
            {
                GUIContent content;
                if (!GUIContentsCache.TryGetValue(name, out content))
                    content = new GUIContent(name);
                content.tooltip = string.Empty;
                content.image = null;
                return content;
            }

            public GUIContent TextContent(string name, Texture2D image)
            {
                GUIContent content = TextContent(name);
                content.image = image;
                return content;
            }

            public GUIContent TextContent(string _name, string _tooltip)
            {
                GUIContent content = TextContent(_name);
                content.tooltip = _tooltip;
                return content;
            }

            public GUIContent TextContent(string name, string tooltip, Texture2D image)
            {
                GUIContent content = TextContent(name);
                content.tooltip = tooltip;
                content.image = image;
                return content;
            }

            public GUIContent NoneContent(string name)
            {
                GUIContent content;
                if (!GUIContentsCache.TryGetValue(name, out content))
                    content = new GUIContent();
                content.tooltip = string.Empty;
                content.image = null;
                return content;
            }
        }

        static GUIContentPool ContentPool = new GUIContentPool();

        public static GUIContent TextContent(string name)
        {
            return ContentPool.TextContent(name);
        }

        public static GUIContent TextContent(string name, Texture2D image)
        {
            return ContentPool.TextContent(name, image);
        }

        public static GUIContent TextContent(string name, string tooltip)
        {
            return ContentPool.TextContent(name, tooltip);
        }

        public static GUIContent TextContent(string name, string tooltip, Texture2D image)
        {
            return ContentPool.TextContent(name, tooltip, image);
        }

        public static GUIContent NoneContent(string name)
        {
            return ContentPool.NoneContent(name);
        }
        #endregion

        #region ContextData
        public interface IContextData { }

        public sealed class ContextData<T> : IContextData { public T value; }

        public class ContextDataCache
        {
            Dictionary<string, IContextData> ContextDatas = new Dictionary<string, IContextData>();

            public bool TryGetContextData<T>(string key, out ContextData<T> contextData)
            {
                if (ContextDatas.TryGetValue(key, out IContextData _data))
                {
                    if (_data is ContextData<T> _t_data)
                    {
                        contextData = _t_data;
                        return true;
                    }
                }
                contextData = new ContextData<T>();
                ContextDatas[key] = contextData;
                return false;
            }

            public ContextData<T> GetContextData<T>(string key, T @default = default)
            {
                if (ContextDatas.TryGetValue(key, out IContextData _data))
                {
                    if (_data is ContextData<T> _t_data)
                    {
                        return _t_data;
                    }
                }
                var contextData = new ContextData<T>();
                contextData.value = @default;
                ContextDatas[key] = contextData;
                return contextData;
            }

            public void RemoveContextData(string key)
            {
                ContextDatas.Remove(key);
            }
        }

        static ContextDataCache ContextDatas = new ContextDataCache();

        public static bool TryGetContextData<T>(string key, out ContextData<T> contextData)
        {
            return ContextDatas.TryGetContextData(key, out contextData);
        }

        public static ContextData<T> GetContextData<T>(string key, T @default = default)
        {
            return ContextDatas.GetContextData(key, @default);
        }

        public static void RemoveContextData(string key)
        {
            ContextDatas.RemoveContextData(key);
        }
        #endregion
    }
}