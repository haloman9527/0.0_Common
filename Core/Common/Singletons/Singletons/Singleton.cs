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
 *  Blog: https://www.mindgear.net/
 *
 */

#endregion

using System;

namespace CZToolKit
{
    [Serializable]
    public abstract class Singleton<T> : ISingleton where T : Singleton<T>, new()
    {
        #region Static

        private static T instance;

        public static T Instance
        {
            get { return instance; }
        }

        public static bool IsInitialized()
        {
            return instance != null;
        }

        #endregion

        private bool isDisposed;

        public bool IsDisposed => this.isDisposed;

        public void Register()
        {
            if (instance != null)
                throw new Exception($"singleton register twice! {typeof(T).Name}");

            instance = (T)this;
        }

        public void Dispose()
        {
            if (this.isDisposed)
                return;

            this.isDisposed = true;
            if (instance is ISingletonDestory iSingletonDestory)
                iSingletonDestory.Destroy();
            instance = null;
        }
    }
}