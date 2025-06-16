#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Atom.UnityEditors
{
    public static class MenuEx
    {
        
#if HALOMAN
        [MenuItem("Tools/Atom/Path Print/DataPath")]
#endif
        public static void PrintDataPath()
        {
            Debug.Log(Application.dataPath);
        }

#if HALOMAN
        [MenuItem("Tools/Atom/Path Print/StreamingAssetsPath")]
#endif
        public static void PrintStreamingAssetsPath()
        {
            Debug.Log(Application.streamingAssetsPath);
        }

#if HALOMAN
        [MenuItem("Tools/Atom/Path Print/PersistentDataPath")]
#endif
        public static void PrintPersistentDataPath()
        {
            Debug.Log(Application.persistentDataPath);
        }

#if HALOMAN
        [MenuItem("Tools/Atom/Path Print/TemporaryCachePath")]
#endif
        public static void PrintTemporaryCachePath()
        {
            Debug.Log(Application.temporaryCachePath);
        }

#if HALOMAN
        [MenuItem("Tools/Atom/Path Print/ComsoleLogPath")]
#endif
        public static void PrintComsoleLogPath()
        {
            Debug.Log(Application.consoleLogPath);
        }

#if HALOMAN
        [MenuItem("Assets/Copy Paths", priority = 100)]
#endif
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

#if HALOMAN
        [MenuItem("Assets/Print Paths", priority = 100)]
#endif
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