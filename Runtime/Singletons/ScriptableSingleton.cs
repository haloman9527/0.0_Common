using UnityEngine;

namespace CZToolKit.Core.Singletons
{
#if ODIN_INSPECTOR
    public abstract class ScriptableSingleton<T> : Sirenix.OdinInspector.SerializedScriptableObject where T : ScriptableSingleton<T>
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
                    T[] ts = Resources.LoadAll<T>("");
                    if (ts.Length > 0)
                    {
#if UNITY_EDITOR
                        Debug.Log("Created:" + typeof(T).Name);
#endif
                        m_Instance = ts[0];
                        m_Instance.OnInitialize();
                    }
                }
                return m_Instance;
            }
        }

        public static T Initialize()
        {
            return Instance;
        }

        protected virtual void OnInitialize() { }

        protected virtual void OnClean() { }

        public static void Clean()
        {
            if (m_Instance != null)
            {
                m_Instance.OnClean();
                m_Instance = null;
            }
        }
    }
}

