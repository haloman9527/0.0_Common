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

namespace CZToolKit.RX
{
    class Subscribe<T> : IObserver<T>
    {
        readonly Action<T> onNext;
        readonly Action<Exception> onError;
        readonly Action onCompleted;

        public Subscribe()
        {
        }

        public Subscribe(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            this.onNext = onNext;
            this.onError = onError;
            this.onCompleted = onCompleted;
        }

        public Subscribe(Action<T> onNext) : this(onNext, null, null)
        {
        }

        public Subscribe(Action<T> onNext, Action<Exception> onError) : this(onNext, onError, null)
        {
        }

        public Subscribe(Action<T> onNext, Action onCompleted) : this(onNext, null, onCompleted)
        {
        }

        public void OnNext(T value)
        {
            onNext?.Invoke(value);
        }

        public void OnError(Exception error)
        {
            onError?.Invoke(error);
        }

        public void OnCompleted()
        {
            onCompleted?.Invoke();
        }
    }

    public static partial class ReactiveExtension
    {
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