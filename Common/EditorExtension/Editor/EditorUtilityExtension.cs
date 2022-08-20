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
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CZToolKit.Core.Editors
{
    public static partial class EditorUtilityExtension
    {
        public static MonoScript FindScriptFromType(Type type, Func<MonoScript, bool> pattern = null, bool compareTypeName = true)
        {
            string findStr = "t:script " + (compareTypeName ? type.Name : "");
            var scriptGUIDs = AssetDatabase.FindAssets(findStr);
            foreach (var scriptGUID in scriptGUIDs)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(scriptGUID);
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);

                if (script != null)
                {
                    if (pattern == null || pattern(script))
                        return script;
                }
            }
            return null;
        }

        public static IEnumerable<MonoScript> FindAllScriptFromType(Type type, Func<MonoScript, bool> pattern = null, bool compareTypeName = true)
        {
            string findStr = "t:script " + (compareTypeName ? type.Name : "");
            var scriptGUIDs = AssetDatabase.FindAssets(findStr);
            foreach (var scriptGUID in scriptGUIDs)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(scriptGUID);
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
                if (script != null)
                {
                    if (pattern == null || pattern(script))
                        yield return script;
                }
            }
        }

        /// <summary> 添加宏定义 </summary>
        /// <param name="define"></param>
        /// <param name="targetGroup"></param>
        public static void AddDefine(string define, BuildTargetGroup targetGroup = BuildTargetGroup.Standalone)
        {
            string s = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            List<string> defines = new List<string>(s.Split(';'));
            if (!defines.Contains(define))
            {
                defines.Add(define);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, string.Join(";", defines.ToArray()));
            }
        }

        /// <summary> 移除宏定义 </summary>
        /// <param name="define"></param>
        /// <param name="targetGroup"></param>
        public static void RemoveDefine(string define, BuildTargetGroup targetGroup = BuildTargetGroup.Standalone)
        {
            string s = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            List<string> defines = new List<string>(s.Split(';'));
            if (defines.Contains(define))
            {
                defines.Remove(define);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, string.Join(";", defines.ToArray()));
            }
        }

        /// <summary> 绝对路径转Assets路径 </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string AbsolutePathToAssetPath(string path)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                int startIndex = path.IndexOf("Assets\\");
                if (startIndex < 0) return path;

                return path.Substring(startIndex);
            }
            else
            {
                int startIndex = path.IndexOf("Assets/");
                if (startIndex < 0) return path;

                return path.Substring(startIndex);
            }
        }
    }
}
#endif