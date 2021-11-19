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
        static readonly object _lock = new object();

        /// <summary> 单例对象 </summary>
        static T _instance;

        /// <summary> 单例对象属性，自动创建 </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            GameObject go = GameObject.Find(typeof(T).ToString());
                            if (go == null)
                            {
                                go = new GameObject(typeof(T).Name);
                                _instance = go.AddComponent<T>();
                            }
                            else
                            {
                                _instance = go.gameObject.GetComponent<T>();
                                if (_instance == null)
                                    _instance = go.gameObject.AddComponent<T>();
                            }
                            DontDestroyOnLoad(go);
                        }
                    }
                }
                return _instance;
            }
        }

        public static bool IsNull { get { return _instance == null; } }

        public static void Initialize()
        {
            _Get();
            T _Get() { return Instance; }
        }

        public static void Destroy()
        {
            if (_instance != null)
            {
                _instance.OnBeforeDestroy();
                Destroy(_instance.gameObject);
                _instance = null;
            }
        }

        protected virtual void Awake() { _instance = this as T; }

        protected virtual void OnBeforeDestroy() { }
    }
}
