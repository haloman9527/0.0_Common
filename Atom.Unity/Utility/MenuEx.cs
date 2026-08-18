#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Atom.UnityEditors
{
    public static class MenuEx
    {
        [MenuItem("Tools/Atom/Path Print/DataPath")]
        public static void PrintDataPath()
        {
            Debug.Log(Application.dataPath);
        }

        [MenuItem("Tools/Atom/Path Print/StreamingAssetsPath")]
        public static void PrintStreamingAssetsPath()
        {
            Debug.Log(Application.streamingAssetsPath);
        }

        [MenuItem("Tools/Atom/Path Print/PersistentDataPath")]
        public static void PrintPersistentDataPath()
        {
            Debug.Log(Application.persistentDataPath);
        }

        [MenuItem("Tools/Atom/Path Print/TemporaryCachePath")]
        public static void PrintTemporaryCachePath()
        {
            Debug.Log(Application.temporaryCachePath);
        }

        [MenuItem("Tools/Atom/Path Print/ComsoleLogPath")]
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