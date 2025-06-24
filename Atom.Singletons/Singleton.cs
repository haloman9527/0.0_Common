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

        private static object s_Lock = new object();
        private static T s_Instance;

        public static T Instance
        {
            get
            {
                if (s_Instance != null)
                    return s_Instance;
                
                lock (@s_Lock)
                {
                    if (s_Instance == null)
                        SingletonEntry.RegisterSingleton(new T());
                }

                return s_Instance;
            }
        }

        public static bool IsInitialized()
        {
            return s_Instance != null;
        }

        #endregion

        private bool m_IsDisposed;

        public bool IsDisposed
        {
            get { return m_IsDisposed; }
        }

        public void Register()
        {
            if (s_Instance != null)
                throw new Exception($"singleton register twice! {TypeCache<T>.TYPE.Name}");

            s_Instance = (T)this;
        }

        public void Dispose()
        {
            if (this.m_IsDisposed)
                return;

            this.m_IsDisposed = true;
            if (this is ISingletonDestroy iSingletonDestory)
                iSingletonDestory.Destroy();
            if (this == s_Instance)
                s_Instance = null;
        }
    }
}