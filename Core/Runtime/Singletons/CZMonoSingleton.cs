#region 注 释
/***
 *
 *  Title:
 *      主题: Mono单例基类
 *  Description:
 *      功能: 自动创建单例对象
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
    [DefaultExecutionOrder(0)]
#if ODIN_INSPECTOR
    public class CZMonoSingleton<T> : Sirenix.OdinInspector.SerializedMonoBehaviour where T : CZMonoSingleton<T>
#else
    public class CZMonoSingleton<T> : MonoBehaviour where T : CZMonoSingleton<T>
#endif
    {
        /// <summary> 线程锁 </summary>
        private static readonly object m_Lock = new object();

        /// <summary> 单例对象 </summary>
        private static T m_Instance;

        /// <summary> 单例对象属性，自动创建 </summary>
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
                            GameObject go = GameObject.Find(typeof(T).ToString());
                            if (go == null)
                            {
                                go = new GameObject(typeof(T).Name);
                                m_Instance = go.AddComponent<T>();
                            }
                            else
                            {
                                m_Instance = go.gameObject.GetComponent<T>();
                                if (m_Instance == null)
                                    m_Instance = go.gameObject.AddComponent<T>();
                            }
                            DontDestroyOnLoad(go);
                        }
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
                Destroy(m_Instance.gameObject);
                m_Instance = null;
            }
        }

        protected virtual void Awake() { m_Instance = this as T; }

        protected virtual void OnBeforeDestroy() { }
    }
}
