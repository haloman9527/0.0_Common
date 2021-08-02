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
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace CZToolKit.Core.Editors
{
    public class EditorPrefsEditor : BasicEditorWindow
    {
        struct PrefData
        {
            // 0: int 1: bool 2: string
            public int type;
            public int intData;
            public bool boolData;
            public string stringData;
        }

        [MenuItem("Tools/CZToolKit/EditorPrefs")]
        public static void Open()
        {
            GetWindow<EditorPrefsEditor>();
        }

        public List<KeyValuePair<string, string>> prefs;

        private void OnEnable()
        {
            prefs = new List<KeyValuePair<string, string>>(GetEditorPrefsKeyValuePairAll());
        }

        private void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {

            }
        }

        private static IEnumerable<KeyValuePair<string, string>> GetEditorPrefsKeyValuePairAll()
        {
            var name = @"Software\Unity Technologies\Unity Editor 5.x\";
            using (var registryKey = Registry.CurrentUser.OpenSubKey(name, false))
            {
                foreach (var valueName in registryKey.GetValueNames())
                {
                    var value = registryKey.GetValue(valueName);
                    var key = valueName.Split(new[] { "_h" }, StringSplitOptions.None)[0];

                    if (value is byte[] byteValue)
                    {
                        yield return new KeyValuePair<string, string>(key, Encoding.UTF8.GetString(byteValue));
                    }   
                    else
                    {
                        yield return new KeyValuePair<string, string>(key, value.ToString());
                    }
                }
            }
        }
    }
}
