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
#if UNITY_EDITOR
using System.Collections;
using Moyo.UnityEditors.EditorCoroutine;
using UnityEditor;
using UnityEngine;

namespace Moyo.UnityEditors
{
    public abstract class BaseEditorWindow : EditorWindow
    {
        static GUIStyle csIconStyle;
        static GUIContent csIcon;

        public static GUIStyle CSIconStyle
        {
            get
            {
                if (csIconStyle == null)
                {
                    csIconStyle = new GUIStyle(GUI.skin.button);
                    csIconStyle.padding.left = 0;
                    csIconStyle.padding.right = 0;
                    csIconStyle.padding.top = 0;
                    csIconStyle.padding.bottom = 0;
                }
                return csIconStyle;
            }
        }
        public static GUIContent CSIcon
        {
            get
            {
                if (csIcon == null)
                    csIcon = new GUIContent(EditorGUIUtility.FindTexture("cs Script Icon"), "打开窗口脚本");
                return csIcon;
            }
        }

        MonoScript monoScript;
        MonoScript MonoScript
        {
            get
            {
                if (monoScript == null)
                    monoScript = MonoScript.FromScriptableObject(this);
                return monoScript;
            }
        }

        EditorCoroutineService coroutineMachine;
        EditorCoroutineService CoroutineMachine
        {
            get
            {
                if (coroutineMachine == null)
                    coroutineMachine = new EditorCoroutineService();
                return coroutineMachine;
            }
        }
        
        private void ShowButton(Rect rect)
        {
#if UNITY_6000_0_OR_NEWER
            rect.x = 0;
            rect.y = 5;
            rect.height = 20;
            rect.width = position.width - 25;
#else
            rect.x = 0;
            rect.width = position.width - 60;
#endif
            GUILayout.BeginArea(rect);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            OnShowButton(rect);
            
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        protected virtual void OnShowButton(Rect rect)
        {
            if (GUILayout.Button(CSIcon, (GUIStyle)"IconButton"))
                AssetDatabase.OpenAsset(MonoScript);
        }

        protected virtual void Update()
        {
            CoroutineMachine.Update();
        }

        public EditorCoroutine.EditorCoroutine StartCoroutine(IEnumerator enumerator)
        {
            return CoroutineMachine.StartCoroutine(enumerator);
        }

        public void StopCoroutine(EditorCoroutine.EditorCoroutine coroutine)
        {
            CoroutineMachine.StopCoroutine(coroutine);
        }
    }
}
#endif