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
        private static readonly object m_Lock = new object();

        private static T m_Instance;

        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    if (m_Instance == null)
                    {
                        m_Instance = Resources.Load<T>(typeof(T).Name);

#if UNITY_EDITOR
                        if (m_Instance == null)
                        {
                            foreach (var guid in UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T).Name}"))
                            {
                                m_Instance = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid));
                                if (m_Instance != null)
                                    break;
                            }
                        }

                        if (m_Instance == null)
                        {
                            m_Instance = CreateInstance<T>();
                            if (!System.IO.Directory.Exists($"Assets/{typeof(T).Name}"))
                                System.IO.Directory.CreateDirectory($"Assets/{typeof(T).Name}");
                            UnityEditor.AssetDatabase.CreateAsset(m_Instance, $"Assets/{typeof(T).Name}/{typeof(T).Name}.asset");
                        }
#else
                        T[] ts = Resources.LoadAll<T>(typeof(T).Name);
                        if (ts.Length > 0)
                            m_Instance = ts[0];
#endif
                    }
                }
                return m_Instance;
            }
        }

        public static bool IsNull { get { return m_Instance == null; } }

        public static void Initialize()
        {
            _Get();
            T _Get() { return Instance; }
        }

        protected virtual void OnEnable()
        {
            m_Instance = this as T;
        }

        public static void Destroy()
        {
            if (m_Instance != null)
            {
                m_Instance.OnBeforeDestroy();
                m_Instance = null;
            }
        }

        protected virtual void OnBeforeDestroy() { }

    }
}

