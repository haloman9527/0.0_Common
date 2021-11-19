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
        private static readonly object _lock = new object();

        /// <summary> 单例对象 </summary>
        private static T _instance;

        /// <summary> 单例对象属性 </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new T();
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
                _instance = null;
            }
        }

        public CZNormalSingleton() { _instance = this as T; }

        protected virtual void OnBeforeDestroy() { }
    }
}
