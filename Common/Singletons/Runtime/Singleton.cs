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

namespace CZToolKit.Common.Singletons
{
    public class Singleton<T> where T : Singleton<T>, new()
    {
        private static readonly object @lock = new object();

        public static T s_Instance { get; protected set; }

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

        public static void Initialize()
        {
            if (s_Instance != null)
                return;
            s_Instance = new T();
        }
    }
}