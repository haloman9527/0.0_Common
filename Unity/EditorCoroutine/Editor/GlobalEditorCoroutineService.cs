#if UNITY_EDITOR
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
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Moyo.UnityEditors.EditorCoroutine
{
    [InitializeOnLoad]
    public class GlobalEditorCoroutineService
    {
        public class GlobalEditorCoroutineSetting
        {
            public bool open = false;
        }
        
        private static GlobalEditorCoroutineSetting s_Settings;
        private static EditorCoroutineService s_CoroutineService =   new EditorCoroutineService();

        static GlobalEditorCoroutineSetting Settings
        {
            get
            {
                if (s_Settings == null)
                {
                    if (EditorPrefs.HasKey(KEY))
                        s_Settings = JsonUtility.FromJson<GlobalEditorCoroutineSetting>(EditorPrefs.GetString(KEY));
                    else
                        s_Settings = new GlobalEditorCoroutineSetting();
                }
                return s_Settings;
            }
        }
        
        #region Perference
        public const string NAME = nameof(GlobalEditorCoroutineService);
        public const string KEY = "GlobalEditorCoroutine.Settings";

#if UNITY_2019_1_OR_NEWER && HALOMAN
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            SettingsProvider provider = new SettingsProvider("Preferences/" + NAME, SettingsScope.User)
            {
                guiHandler = (searchContext) => { PreferencesGUI(); },
            };
            return provider;
        }
#endif

#if !UNITY_2019_1_OR_NEWER && HALOMAN
        [PreferenceItem(Name)]
#endif
        private static void PreferencesGUI()
        {
            EditorGUI.BeginChangeCheck();
            Settings.open = EditorGUILayout.Toggle("Open", Settings.open);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetString(KEY, JsonUtility.ToJson(Settings));
                InitGlobalCoroutine();

            }
        }
        #endregion

        static GlobalEditorCoroutineService()
        {
            InitGlobalCoroutine();
        }

        static void InitGlobalCoroutine()
        {
            if (Settings.open)
                EditorApplication.update += Update;
            else
                EditorApplication.update -= Update;
        }

        static void Update()
        {
            s_CoroutineService.Update();
        }

        public static ICoroutine StartCoroutine(IEnumerator enumerator)
        {
            return s_CoroutineService.StartCoroutine(enumerator);
        }

        public static void StopCoroutine(EditorCoroutine coroutine)
        {
            s_CoroutineService.StopCoroutine(coroutine);
        }
    }
}
#endif