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
using UnityEditor;
using UnityEngine;

namespace CZToolKit.Core.Editors
{

    public class PathDebugger
    {
        [MenuItem("Tools/CZToolKit/Path Debugger/DataPath")]
        public static void DebugDataPath()
        {
            Debug.Log(Application.dataPath);
        }

        [MenuItem("Tools/CZToolKit/Path Debugger/StreamingAssetsPath")]
        public static void DebugStreamingAssetsPath()
        {
            Debug.Log(Application.streamingAssetsPath);
        }

        [MenuItem("Tools/CZToolKit/Path Debugger/PersistentDataPath")]
        public static void DebugPersistentDataPath()
        {
            Debug.Log(Application.persistentDataPath);
        }

        [MenuItem("Tools/CZToolKit/Path Debugger/TemporaryCachePath")]
        public static void DebugTemporaryCachePath()
        {
            Debug.Log(Application.temporaryCachePath);
        }

        [MenuItem("Tools/CZToolKit/Path Debugger/ComsoleLogPath")]
        public static void DebugComsoleLogPath()
        {
            Debug.Log(Application.consoleLogPath);
        }
    }
}
#endif