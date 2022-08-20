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

namespace CZToolKit.Core.Singletons
{
    public class CommonSingleton<T> where T : CommonSingleton<T>, new()
    {
        /// <summary> 线程锁 </summary>
        private static readonly object _lock = new object();

        /// <summary> 单例对象 </summary>
        public static T instance
        {
            get;
            protected set;
        }

        /// <summary> 单例对象属性 </summary>
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
            instance = new T();
        }
    }
}
