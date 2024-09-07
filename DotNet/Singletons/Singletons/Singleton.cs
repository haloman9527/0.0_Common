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

namespace CZToolKit
{
    [Serializable]
    public abstract class Singleton<T> : ISingleton where T : Singleton<T>
    {
        #region Static

        private static T s_Instance;

        public static T Instance => s_Instance;

        public static bool IsInitialized()
        {
            return s_Instance != null;
        }

        #endregion

        private bool isDisposed;

        public bool IsDisposed => this.isDisposed;

        public void Register()
        {
            if (s_Instance != null)
                throw new Exception($"singleton register twice! {typeof(T).Name}");

            s_Instance = (T)this;
        }

        public void Dispose()
        {
            if (this.isDisposed)
                return;

            this.isDisposed = true;
            if (s_Instance is ISingletonDestory iSingletonDestory)
                iSingletonDestory.Destroy();
            s_Instance = null;
        }
    }
}