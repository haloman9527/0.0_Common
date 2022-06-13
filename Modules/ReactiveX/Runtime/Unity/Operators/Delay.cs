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
    public class Delay<T> : Operator<T>
    {
        float delay;
        Coroutine coroutine;

        public Delay(IObservable<T> src, float delay) : base(src)
        {
            this.delay = delay;
        }

        public override void OnNext(T value)
        {
            coroutine = MainThreadDispatcher.Instance.StartCoroutine(DDelay(value));
        }

        IEnumerator DDelay(T value)
        {
            yield return new WaitForSeconds(delay);
            observer.OnNext(value);
        }

        public override void OnDispose()
        {
            if (MainThreadDispatcher.instance != null)
                MainThreadDispatcher.Instance.StopCoroutine(coroutine);
        }
    }

    public class SelfDelay<T> : Operator<T> where T : MonoBehaviour
    {
        float delay;
        Coroutine coroutine;
        T obs;

        public SelfDelay(IObservable<T> src, float delay) : base(src)
        {
            this.delay = delay;
        }

        public override void OnNext(T value)
        {
            obs = value;
            coroutine = obs.StartCoroutine(DDelay(obs));
        }

        IEnumerator DDelay(T _value)
        {
            yield return new WaitForSeconds(delay);
            observer.OnNext(_value);
        }

        public override void OnDispose()
        {
            obs.StopCoroutine(coroutine);
        }
    }
    public static partial class Extension
    {
        /// <summary> 通过协程实现的延迟 </summary>
        public static IObservable<T> Delay<T>(this IObservable<T> _src, float delayTime)
        {
            return new Delay<T>(_src, delayTime);
        }

        public static IObservable<T> SelfDelay<T>(this IObservable<T> _src, float delayTime) where T : MonoBehaviour
        {
            return new SelfDelay<T>(_src, delayTime);
        }
    }
}