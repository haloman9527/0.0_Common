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
    public abstract class CZScriptableSingleton<T> : Sirenix.OdinInspector.SerializedScriptableObject where T : CZScriptableSingleton<T>
#else
    public abstract class CZScriptableSingleton<T> : ScriptableObject where T : CZScriptableSingleton<T>
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
                            m_Instance = ScriptableObject.CreateInstance<T>();
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

