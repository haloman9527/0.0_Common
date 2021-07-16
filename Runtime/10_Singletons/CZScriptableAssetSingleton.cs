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
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DefaultAssetPathAttribute : Attribute
    {
        public string DefaultAssetPath;
        public DefaultAssetPathAttribute(string _folder)
        {
            DefaultAssetPath = _folder;
        }
    }

#if ODIN_INSPECTOR
    public abstract class CZScriptableAssetSingleton<T> : Sirenix.OdinInspector.SerializedScriptableObject where T : CZScriptableAssetSingleton<T>
#else
    public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
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
                    lock (m_Lock)
                    {
                        if (m_Instance == null)
                        {
                            m_Instance = Resources.Load<T>(typeof(T).Name);

#if UNITY_EDITOR
                            string path = null;
                            // 查找特性
                            if (Utility_Attribute.TryGetTypeAttribute(typeof(T), out DefaultAssetPathAttribute defaultAssetPath))
                            {
                                m_Instance = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(defaultAssetPath.DefaultAssetPath);
                                path = defaultAssetPath.DefaultAssetPath;
                            }
                            else
                                path = $"Assets/{nameof(T)}.Asset";

                            if (m_Instance == null)
                            {
                                foreach (var guid in UnityEditor.AssetDatabase.FindAssets($"t:{nameof(T)}"))
                                {
                                    m_Instance = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid));
                                    if (m_Instance != null)
                                        break;
                                }
                            }

                            if (m_Instance == null)
                            {
                                m_Instance = CreateInstance<T>();
                                UnityEditor.AssetDatabase.CreateAsset(m_Instance, path);
                            }
#else

                        T[] ts = Resources.LoadAll<T>(typeof(T).Name);
                        if (ts.Length > 0)
                            m_Instance = ts[0];
#endif
                            if (m_Instance != null)
                                m_Instance.OnInitialize();
                        }
                    }
                }
                return m_Instance;
            }
        }

        public static bool IsNull { get { return m_Instance == null; } }

        public static T Initialize() { return Instance; }

        public static void Clean()
        {
            if (m_Instance != null)
            {
                m_Instance.OnClean();
                m_Instance = null;
            }
        }

        protected virtual void OnInitialize() { }

        protected virtual void OnClean() { }

    }
}

