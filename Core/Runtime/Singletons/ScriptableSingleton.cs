#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 半只龙虾人
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */
#endregion
using UnityEngine;

namespace CZToolKit.Core.Singletons
{
#if ODIN_INSPECTOR
    public abstract class ScriptableSingleton<T> : Sirenix.OdinInspector.SerializedScriptableObject where T : ScriptableSingleton<T>
#else
    public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
#endif
    {
        /// <summary> 线程锁 </summary>
        private static readonly object _lock = new object();

        /// <summary> 单例对象 </summary>
        public static T instance
        {
            get;
            protected set;
        }

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
            instance = ScriptableObject.CreateInstance<T>();
        }

        public static void Destroy()
        {
            if (instance != null)
            {
                instance.OnBeforeDestroy();
                instance = null;
            }
        }

        protected virtual void OnBeforeDestroy() { }
    }
}

