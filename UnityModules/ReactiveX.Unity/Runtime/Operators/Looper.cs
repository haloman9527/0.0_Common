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
 *  Github: https://github.com/haloman9527
 *  Blog: https://www.haloman.net/
 *
 */
#endregion
using System;
using System.Collections;
using UnityEngine;

namespace CZToolKit.RX
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

            Next(value);

            WaitForSeconds seconds = new WaitForSeconds(this.interval);
            int currentLoops = 0;
            while (this.loopTime < 0 || this.loopTime > currentLoops++)
            {
                yield return seconds;
                Next(value);
            }
        }

        public override void OnDispose()
        {
            if (MainThreadDispatcher.IsInitialized())
                MainThreadDispatcher.Instance.StopCoroutine(coroutine);
        }
    }

    public static partial class ReactiveExtension
    {
        public static IObservable<T> Looper<T>(this IObservable<T> src, float delay, float interval, int loopTimes = -1)
        {
            return new Looper<T>(src, delay, interval, loopTimes);
        }
    }
}
