﻿using UnityEngine;

namespace CZToolKit.Core.Singletons
{
#if ODIN_INSPECTOR
    public abstract class CZScriptableSingleton<T> : Sirenix.OdinInspector.SerializedScriptableObject where T : CZScriptableSingleton<T>
#else
    public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
#endif
    {
        private static T m_Instance;

        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = Resources.Load<T>(typeof(T).Name);
                    if (m_Instance == null)
                    {
                        T[] ts = Resources.LoadAll<T>(typeof(T).Name);
                        if (ts.Length > 0)
                        {
#if UNITY_EDITOR
                            Debug.Log("Created:" + typeof(T).Name);
#endif
                            m_Instance = ts[0];
                        }
                    }

                    m_Instance.OnInitialize();
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
