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

namespace CZToolKit.Core.ReactiveX
{
    public static partial class Extension
    {
        public static Observable<T> ToObservable<T>(this T src)
        {
            return new Observable<T>(src);
        }

        public static IDisposable Subscribe<T>(this IObservable<T> src)
        {
            return src.Subscribe(new Subscribe<T>());
        }

        public static IDisposable Subscribe<T>(this IObservable<T> src, Action<T> onNext)
        {
            return src.Subscribe(new Subscribe<T>(onNext));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> src, Action<T> onNext, Action<Exception> onError)
        {
            return src.Subscribe(new Subscribe<T>(onNext, onError));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> src, Action<T> onNext, Action onCompleted)
        {
            return src.Subscribe(new Subscribe<T>(onNext, onCompleted));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> src, Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            return src.Subscribe(new Subscribe<T>(onNext, onError, onCompleted));
        }
    }
}
