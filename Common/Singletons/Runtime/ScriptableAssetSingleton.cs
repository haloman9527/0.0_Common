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

namespace CZToolKit.Common.Singletons
{
#if ODIN_INSPECTOR
    public abstract class ScriptableAssetSingleton<T> : Sirenix.OdinInspector.SerializedScriptableObject where T : ScriptableAssetSingleton<T>
#else
    public abstract class ScriptableAssetSingleton<T> : UnityEngine.ScriptableObject where T : ScriptableAssetSingleton<T>
#endif
    {
        /// <summary> 线程锁 </summary>
        private static readonly object @lock = new object();

        /// <summary> 单例对象 </summary>
        public static T s_Instance
        {
            get;
            protected set;
        }

        public static T Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    lock (@lock)
                    {
                        if (s_Instance == null)
                        {
                            Initialize();
                        }
                    }
                }
                return s_Instance;
            }
        }

        public static void Initialize()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                throw new System.Exception();
#endif
            
            if (s_Instance != null)
                return;

#if UNITY_EDITOR
            foreach (var guid in UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T).Name}"))
            {
                s_Instance = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid));
                if (s_Instance != null)
                    break;
            }

            if (s_Instance == null)
            {
                s_Instance = CreateInstance<T>();
                if (!System.IO.Directory.Exists($"Assets/{typeof(T).Name}"))
                    System.IO.Directory.CreateDirectory($"Assets/{typeof(T).Name}");
                UnityEditor.AssetDatabase.CreateAsset(s_Instance, $"Assets/{typeof(T).Name}/{typeof(T).Name}.asset");
            }
#else
            T[] ts = Resources.LoadAll<T>(typeof(T).Name);
            if (ts.Length > 0)
                instance = ts[0];
#endif
        }
    }
}

