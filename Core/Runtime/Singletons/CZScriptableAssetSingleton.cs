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
using System;
using UnityEngine;

namespace CZToolKit.Core.Singletons
{
#if ODIN_INSPECTOR
    public abstract class CZScriptableAssetSingleton<T> : Sirenix.OdinInspector.SerializedScriptableObject where T : CZScriptableAssetSingleton<T>
#else
    public abstract class CZScriptableAssetSingleton<T> : ScriptableObject where T : CZScriptableAssetSingleton<T>
#endif
    {
        /// <summary> 线程锁 </summary>
        private static readonly object _lock = new object();

        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (_instance == null)
                    {
                        _instance = Resources.Load<T>(typeof(T).Name);

#if UNITY_EDITOR
                        if (_instance == null)
                        {
                            foreach (var guid in UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T).Name}"))
                            {
                                _instance = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid));
                                if (_instance != null)
                                    break;
                            }
                        }

                        if (_instance == null)
                        {
                            _instance = CreateInstance<T>();
                            if (!System.IO.Directory.Exists($"Assets/{typeof(T).Name}"))
                                System.IO.Directory.CreateDirectory($"Assets/{typeof(T).Name}");
                            UnityEditor.AssetDatabase.CreateAsset(_instance, $"Assets/{typeof(T).Name}/{typeof(T).Name}.asset");
                        }
#else
                        T[] ts = Resources.LoadAll<T>(typeof(T).Name);
                        if (ts.Length > 0)
                            m_Instance = ts[0];
#endif
                    }
                }
                return _instance;
            }
        }

        public static bool IsNull { get { return _instance == null; } }

        public static void Initialize()
        {
            _Get();
            T _Get() { return Instance; }
        }

        protected virtual void OnEnable()
        {
            _instance = this as T;
        }

        public static void Destroy()
        {
            if (_instance != null)
            {
                _instance.OnBeforeDestroy();
                _instance = null;
            }
        }

        protected virtual void OnBeforeDestroy() { }

    }
}

