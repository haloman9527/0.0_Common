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
using System;
using UnityEngine;

namespace CZToolKit.Core.Singletons
{
#if ODIN_INSPECTOR
    public abstract class CZScriptableSingleton<T> : Sirenix.OdinInspector.SerializedScriptableObject where T : CZScriptableSingleton<T>
#else
    public abstract class CZScriptableSingleton<T> : ScriptableObject where T : CZScriptableSingleton<T>
#endif
    {
        /// <summary> 线程锁 </summary>
        private static readonly object _lock = new object();

        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = ScriptableObject.CreateInstance<T>();
                    }
                }
                return _instance;
            }
        }

        public static bool IsNull { get { return _instance == null; } }

        public static void Initialize()
        {
            _Get();
            T _Get() { return Instance; }
        }

        public static void Destroy()
        {
            if (_instance != null)
            {
                _instance.OnBeforeDestroy();
                _instance = null;
            }
        }

        protected virtual void OnBeforeDestroy() { }

    }
}

