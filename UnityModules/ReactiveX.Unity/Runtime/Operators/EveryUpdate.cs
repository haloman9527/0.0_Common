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
    public enum UpdateType { FixedUpdate, Update, LateUpdate }
    public class EveryUpdate<T> : Operator<T>
    {
        UpdateType updateType = UpdateType.Update;

        public EveryUpdate(IObservable<T> src, UpdateType updateType) : base(src)
        {
            this.updateType = updateType;
        }

        Coroutine coroutine;
        public override void OnNext(T value)
        {
            coroutine = MainThreadDispatcher.Instance.StartCoroutine(Update(() =>
            {
                Next(value);
            }));
        }

        public IEnumerator Update(Action action)
        {
            switch (updateType)
            {
                case UpdateType.FixedUpdate:
                    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
                    while (true)
                    {
                        action();
                        yield return waitForFixedUpdate;
                    }
                case UpdateType.Update:
                    while (true)
                    {
                        action();
                        yield return true;
                    }
                case UpdateType.LateUpdate:
                    WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
                    while (true)
                    {
                        action();
                        yield return waitForEndOfFrame;
                    }
            }
        }

        public override void OnDispose()
        {
            MainThreadDispatcher.Instance.StopCoroutine(coroutine);
        }
    }

    public static partial class ReactiveExtension
    {
        public static IObservable<T> EveryUpdate<T>(this IObservable<T> src, UpdateType updateType = UpdateType.Update)
        {
            return new EveryUpdate<T>(src, updateType);
        }
    }
}