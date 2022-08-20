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
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CZToolKit.Core.Editors
{
    public class EditorCoroutineService : CoroutineService<EditorCoroutine>
    {
        public override EditorCoroutine StartCoroutine(IEnumerator enumerator)
        {
            EditorCoroutine coroutine = new EditorCoroutine(enumerator);
            coroutineQueue.Enqueue(coroutine);
            return coroutine;
        }

        public override void StopCoroutine(EditorCoroutine coroutine)
        {
            coroutine.Stop();
        }
    }

    public class EditorCoroutine : ICoroutine, IYield
    {
        IEnumerator enumerator;
        Dictionary<int, IYield> cache = new Dictionary<int, IYield>();

        public bool IsRunning { get; private set; } = true;
        public double TimeSinceStartup { get; private set; }
        public int FrameSinceStartup { get; private set; }
        public IYield Current { get { return Get(enumerator.Current); } }

        public IYield Get(object source)
        {
            if (source == null)
                return null;
            switch (source)
            {
                case WaitForSeconds wfs:
                    {
                        int hash = wfs.GetHashCode();
                        if (!cache.TryGetValue(hash, out IYield y))
                        {
                            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
                            FieldInfo field = typeof(WaitForSeconds).GetField("m_Seconds", bindingFlags);
                            if (field != null)
                                y = new EditorWaitForSeconds(float.Parse(field.GetValue(wfs).ToString()));
                            else
                                y = null;
                            cache[hash] = y;
                        }
                        return y;
                    }
                default:
                    break;
            }
            return source as IYield;
        }

        public EditorCoroutine(IEnumerator enumerator)
        {
            this.enumerator = enumerator;
        }

        public bool MoveNext()
        {
            TimeSinceStartup = EditorApplication.timeSinceStartup;
            return IsRunning = enumerator.MoveNext();
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public bool Result(ICoroutine coroutine)
        {
            return !IsRunning;
        }
    }
}
#endif