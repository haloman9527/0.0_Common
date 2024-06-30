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
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CZToolKit.Unity
{
    public static class Util_UnityEditor
    {
        public static void OpenDefineSymbols(List<string> defines, BuildTargetGroup targetGroup)
        {
            foreach (var def in defines)
            {
                if (def.Contains(";"))
                    throw new InvalidOperationException();
            }

            var defs = new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(';'));

            foreach (var def in defines)
            {
                if (def.Contains(";"))
                    throw new InvalidOperationException();
                if (defs.Contains(def.Trim()))
                    defs.Add(def.Trim());
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, string.Join(";", defs));
        }

        public static void CloseDefineSymbols(List<string> defines, BuildTargetGroup targetGroup)
        {
            foreach (var def in defines)
            {
                if (def.Contains(";"))
                    throw new InvalidOperationException();
            }

            var defs = new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(';'));

            foreach (var def in defines)
            {
                if (defs.Contains(def.Trim()))
                    defs.Remove(def.Trim());
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, string.Join(";", defs));
        }

        [MenuItem("Tools/CZToolKit/Path Print/DataPath")]
        public static void PrintDataPath()
        {
            Debug.Log(Application.dataPath);
        }

        [MenuItem("Tools/CZToolKit/Path Print/StreamingAssetsPath")]
        public static void PrintStreamingAssetsPath()
        {
            Debug.Log(Application.streamingAssetsPath);
        }

        [MenuItem("Tools/CZToolKit/Path Print/PersistentDataPath")]
        public static void PrintPersistentDataPath()
        {
            Debug.Log(Application.persistentDataPath);
        }

        [MenuItem("Tools/CZToolKit/Path Print/TemporaryCachePath")]
        public static void PrintTemporaryCachePath()
        {
            Debug.Log(Application.temporaryCachePath);
        }

        [MenuItem("Tools/CZToolKit/Path Print/ComsoleLogPath")]
        public static void PrintComsoleLogPath()
        {
            Debug.Log(Application.consoleLogPath);
        }

        [MenuItem("Assets/Copy Paths", priority = 100)]
        public static void CopySelectionPath()
        {
            var count = Selection.objects.Length;
            var paths = new string[count];
            for (int i = 0; i < count; i++)
            {
                paths[i] = AssetDatabase.GetAssetPath(Selection.objects[i]);
            }

            GUIUtility.systemCopyBuffer = string.Join('\n', paths);
        }

        [MenuItem("Assets/Print Paths", priority = 100)]
        public static void PrintSelectionPath()
        {
            var count = Selection.objects.Length;
            var paths = new string[count];
            for (int i = 0; i < count; i++)
            {
                paths[i] = AssetDatabase.GetAssetPath(Selection.objects[i]);
            }

            Debug.Log(string.Join('\n', paths));
        }
    }
}
#endif