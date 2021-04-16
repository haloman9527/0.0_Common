#region <<版 本 注 释>>
/***
 *
 *  Title:""
 *      主题: Mono单例基类
 *  Description:
 *      功能: 自动创建单例对象
 *  
 *  Date:
 *  Version:
 *  Writer: 
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

        public static GameObject mgrParent;

        /// <summary> 单例对象 </summary>
        private static T m_Instance;

        public virtual bool DontDestoryOnLoad { get { return true; } }

        /// <summary> 单例对象属性 </summary>
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
                            mgrParent = GetMgrParent();

                            Transform mgrTrans = mgrParent.transform.Find(typeof(T).ToString());

                            if (mgrTrans == null)
                            {
                                mgrTrans = new GameObject(typeof(T).Name).transform;
                                m_Instance = mgrTrans.gameObject.AddComponent<T>();
                                m_Instance.OnInitialize();
                                mgrTrans.transform.SetParent(mgrParent.transform);
                            }
                            else
                            {
                                m_Instance = mgrTrans.gameObject.GetComponent<T>();
                                if (m_Instance == null)
                                    m_Instance = mgrTrans.gameObject.AddComponent<T>();
                            }
                        }
                    }
                }
                return m_Instance;
            }
        }

        public static bool IsNull { get { return m_Instance == null; } }

        protected virtual void Awake()
        {
            m_Instance = this as T;
            if (DontDestoryOnLoad)
                DontDestroyOnLoad(m_Instance.gameObject);
        }

        public static GameObject GetMgrParent()
        {
            if (mgrParent == null)
            {
                mgrParent = GameObject.Find("CZManagers");
                if (mgrParent == null)
                {
                    mgrParent = new GameObject("CZManagers");
                    DontDestroyOnLoad(mgrParent);
                }
            }
            return mgrParent;
        }

        public static T Initialize()
        {
            return Instance;
        }

        public static void Clean()
        {
            if (m_Instance != null)
            {
                m_Instance.OnClean();
                Destroy(m_Instance.gameObject);
                m_Instance = null;
            }
        }

        protected virtual void OnInitialize() { }

        protected virtual void OnClean() { }
    }
}
