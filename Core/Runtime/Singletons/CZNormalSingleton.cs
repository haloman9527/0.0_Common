#region 注 释
/***
 *
 *  Title:
 *      主题: 单例基类
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
using System;

namespace CZToolKit.Core.Singletons
{
    public class CZNormalSingleton<T> where T : CZNormalSingleton<T>, new()
    {
        /// <summary> 线程锁 </summary>
        private static readonly object m_Lock = new object();

        /// <summary> 单例对象 </summary>
        private static T m_Instance;

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
                            m_Instance = new T();
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

        public CZNormalSingleton() { m_Instance = this as T; }

        protected virtual void OnBeforeDestroy() { }
    }
}
