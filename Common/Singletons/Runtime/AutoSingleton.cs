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

namespace CZToolKit.Common.Singletons
{
    public abstract class AutoSingleton<T> : ISingleton where T : AutoSingleton<T>, new()
    {
        private static object @lock = new object();
        private bool isDisposed;
        
        private static T s_instance;

        public static T Instance
        {
            get
            {
                if (s_instance == null)
                {
                    lock (@lock)
                    {
                        if (s_instance == null)
                        {
                            Game.AddSingleton(new T());
                        }
                    }
                }
                return s_instance;
            }
        }

        void ISingleton.Register()
        {
            if (s_instance != null)
            {
                throw new Exception($"singleton register twice! {typeof (T).Name}");
            }
            s_instance = (T)this;
        }

        void ISingleton.Destroy()
        {
            if (this.isDisposed)
            {
                return;
            }
            this.isDisposed = true;
            
            s_instance.Dispose();
            s_instance = null;
        }

        bool ISingleton.IsDisposed()
        {
            return this.isDisposed;
        }

        public virtual void Dispose()
        {
        }
    }
}