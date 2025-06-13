using System;

namespace Atom
{
    /// <summary>
    /// 自动创建
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : ISingleton where T : Singleton<T>, new()
    {
        #region Static

        private static object @lock = new object();
        private bool isDisposed;

        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance != null)
                    return instance;
                
                lock (@lock)
                {
                    if (instance == null)
                        SingletonEntry.RegisterSingleton(new T());
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
            if (this is ISingletonDestroy iSingletonDestory)
                iSingletonDestory.Destroy();
            if (this == instance)
                instance = null;
        }
    }
}