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
using System.Collections;
using UnityEngine;

namespace CZToolKit.Core.ReactiveX
{
    public class Looper<T> : Operator<T>
    {
        float delay;
        float interval;
        int loopTime;
        Coroutine coroutine;

        public Looper(IObservable<T> src, float delay, float interval, int loopTime) : base(src)
        {
            this.delay = Mathf.Max(0, delay);
            this.interval = Mathf.Max(0, interval);
            this.loopTime = loopTime;
        }

        public override void OnNext(T value)
        {
            coroutine = MainThreadDispatcher.Instance.StartCoroutine(Loop(delay, interval, loopTime, value));
        }

        IEnumerator Loop(float delay, float interval, int loopTime, T value)
        {
            if (this.delay != 0)
                yield return new WaitForSeconds(delay);

            base.OnNext(value);

            WaitForSeconds seconds = new WaitForSeconds(this.interval);
            int currentLoops = 0;
            while (this.loopTime < 0 || this.loopTime > currentLoops++)
            {
                yield return seconds;
                base.OnNext(value);
            }
        }

        public override void OnDispose()
        {
            if (MainThreadDispatcher.instance != null)
                MainThreadDispatcher.Instance.StopCoroutine(coroutine);
        }
    }

    public static partial class Extension
    {
        public static IObservable<T> Looper<T>(this IObservable<T> src, float delay, float interval, int loopTimes = -1)
        {
            return new Looper<T>(src, delay, interval, loopTimes);
        }
    }
}
