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
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        /// <summary> 线程锁 </summary>
        static readonly object _lock = new object();

        /// <summary> 单例对象 </summary>
        public static T instance
        {
            get;
            protected set;
        }

        /// <summary> 单例对象属性，自动创建 </summary>
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (_lock)
                    {
                        Initialize();
                    }
                }
                return instance;
            }
        }

        public static void Initialize()
        {
            if (instance != null)
                return;
            instance = GameObject.FindObjectOfType<T>();
            if (instance == null)
                instance = new GameObject(typeof(T).Name).AddComponent<T>();
            DontDestroyOnLoad(instance.gameObject);
        }

        public static void Destroy()
        {
            if (instance != null)
            {
                instance.OnBeforeDestroy();
                Destroy(instance.gameObject);
                instance = null;
            }
        }

        protected virtual void OnBeforeDestroy() { }
    }
}
