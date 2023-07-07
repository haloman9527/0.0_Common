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

namespace CZToolKit.Common.Singletons
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        /// <summary> 线程锁 </summary>
        static readonly object @lock = new object();

        /// <summary> 单例对象 </summary>
        public static T s_Instance { get; protected set; }

        /// <summary> 单例对象属性，自动创建 </summary>
        public static T Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    lock (@lock)
                    {
                        if (s_Instance == null)
                        {
                            Initialize();
                        }
                    }
                }

                return s_Instance;
            }
        }

        public MonoSingleton()
        {
            
        }

        public static void Initialize()
        {
            if (s_Instance != null)
                return;
            s_Instance = GameObject.FindObjectOfType<T>();
            if (s_Instance == null)
                s_Instance = new GameObject(typeof(T).Name).AddComponent<T>();
            DontDestroyOnLoad(s_Instance.gameObject);
        }
    }
}