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
 *  Github: https://github.com/haloman9527
 *  Blog: https://www.haloman.net/
 *
 */

#endregion

using System;

namespace Atom
{
    public abstract class AutoSingleton<T> : ISingleton where T : AutoSingleton<T>, new()
    {
        #region Static

        private static object @lock = new object();
        private bool isDisposed;

        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (@lock)
                    {
                        if (instance == null)
                        {
                            Game.AddSingleton(new T());
                        }
                    }
                }

                return instance;
            }
        }

        public static bool IsInitialized()
        {
            return instance != null;
        }

        #endregion

        public bool IsDisposed => this.isDisposed;

        public void Register()
        {
            if (instance != null)
                throw new Exception($"singleton register twice! {TypeCache<T>.TYPE.Name}");

            instance = (T)this;
        }

        public void Dispose()
        {
            if (this.isDisposed)
                return;

            this.isDisposed = true;
            if (this is ISingletonDestory iSingletonDestory)
                iSingletonDestory.Destroy();
            if (this == instance)
                instance = null;
        }
    }
}