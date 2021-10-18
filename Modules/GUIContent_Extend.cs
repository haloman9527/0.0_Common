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
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CZToolKit.Core.Editors
{
    public class Localization
    {
        int language;
        string[] languages;
        Dictionary<string, string[]> texts = new Dictionary<string, string[]>();

        public event Action onLanguageChanged;

        public string[] Languages
        {
            get { return languages; }
        }
        public int Language
        {
            get { return language; }
            set
            {
                if (language == value) return;
                language = value;
                onLanguageChanged?.Invoke();
            }
        }

        public Localization(string _text)
        {
            bool firstLine = true;
            CSVLoader.DeserializeEachLine(_text, _fields =>
            {
                if (firstLine)
                {
                    languages = _fields.Skip(1).ToArray();
                    firstLine = false;
                }
                else
                    texts[_fields[0]] = _fields.Skip(1).ToArray();
            });
        }

        public string GetText(string _key)
        {
            if (texts.TryGetValue(_key, out string[] _texts))
            {
                if (_texts.Length > language)
                    return _texts[language];
            }
            return _key;
        }
        Dictionary<string, GUIContent_Extend> contents = new Dictionary<string, GUIContent_Extend>();
        public GUIContent GetGUIContent(string _key)
        {
            if (contents.TryGetValue(_key, out GUIContent_Extend content))
                return content;
            return contents[_key] = content = new GUIContent_Extend(_key, this);
        }
    }

    public class GUIContent_Extend : GUIContent
    {
        string key;
        Localization dataSource;

        public string Key
        {
            get { return key; }
            set
            {
                if (key == value) return;
                key = value;
                Refresh();
            }
        }

        public GUIContent_Extend(Localization _dataSource)
        {
            dataSource = _dataSource;
            dataSource.onLanguageChanged += Refresh;
        }

        public GUIContent_Extend(string _key, Localization _owner) : base(_owner.GetText(_key))
        {
            key = _key;
            dataSource = _owner;
            dataSource.onLanguageChanged += Refresh;
        }

        public GUIContent_Extend(Texture image, Localization _owner) : base(image)
        {
            dataSource = _owner;
            dataSource.onLanguageChanged += Refresh;
        }

        public GUIContent_Extend(string _key, Texture image, Localization _owner) : base(_owner.GetText(_key), image)
        {
            key = _key;
            dataSource = _owner;
            dataSource.onLanguageChanged += Refresh;
        }

        void Refresh()
        {
            text = dataSource.GetText(key);
        }
    }
}
#endif