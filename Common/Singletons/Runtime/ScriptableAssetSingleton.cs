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
using UnityEngine;

namespace CZToolKit.Core.Singletons
{
#if ODIN_INSPECTOR
    public abstract class ScriptableAssetSingleton<T> : Sirenix.OdinInspector.SerializedScriptableObject where T : ScriptableAssetSingleton<T>
#else
    public abstract class ScriptableAssetSingleton<T> : ScriptableObject where T : ScriptableAssetSingleton<T>
#endif
    {
        /// <summary> 线程锁 </summary>
        private static readonly object _lock = new object();

        /// <summary> 单例对象 </summary>
        public static T instance
        {
            get;
            protected set;
        }

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (_lock)
                    {
                        Initialize();
                    }
                }
                return instance;
            }
        }

        public static void Initialize()
        {
            if (instance != null)
                return;

#if UNITY_EDITOR
            foreach (var guid in UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T).Name}"))
            {
                instance = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid));
                if (instance != null)
                    break;
            }

            if (instance == null)
            {
                instance = CreateInstance<T>();
                if (!System.IO.Directory.Exists($"Assets/{typeof(T).Name}"))
                    System.IO.Directory.CreateDirectory($"Assets/{typeof(T).Name}");
                UnityEditor.AssetDatabase.CreateAsset(instance, $"Assets/{typeof(T).Name}/{typeof(T).Name}.asset");
            }
#else
            T[] ts = Resources.LoadAll<T>(typeof(T).Name);
            if (ts.Length > 0)
                instance = ts[0];
#endif
        }

        public static void Destroy()
        {
            if (instance != null)
            {
                instance.OnBeforeDestroy();
                instance = null;
            }
        }

        protected virtual void OnBeforeDestroy() { }

    }
}

