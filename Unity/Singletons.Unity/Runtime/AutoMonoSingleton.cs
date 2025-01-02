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
 *  Github: https://github.com/haloman9527
 *  Blog: https://www.haloman.net/
 *
 */

#endregion

using UnityEngine;

namespace Moyo.Unity
{
    public class AutoMonoSingleton<T> : MonoBehaviour, ISingleton where T : AutoMonoSingleton<T>
    {
        #region Static

        private static readonly object @lock = new object();
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (@lock)
                    {
                        Initialize();
                    }
                }

                return instance;
            }
        }

        public static bool IsInitialized()
        {
            return instance != null;
        }

        public static void Initialize()
        {
            if (instance != null)
                return;
            instance = GameObject.FindObjectOfType<T>();
            if (instance == null)
                instance = new GameObject(TypeCache<T>.TYPE.Name).AddComponent<T>();
            Game.AddSingleton(instance);
        }

        #endregion

        private bool isDisposed;

        public bool IsDisposed => this.isDisposed;

        public void Register()
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }

        public void Dispose()
        {
            if (this.isDisposed)
                return;

            this.isDisposed = true;
            if (instance is ISingletonDestory iSingletonDestory)
                iSingletonDestory.Destroy();
            Destroy(gameObject);
            instance = null;
        }
    }
}