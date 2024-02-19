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
    public class Observable<T> : IDisposable, IObservable<T>
    {
        private readonly T value;
        private IObserver<T> observer;

        public Observable(T value)
        {
            this.value = value;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            this.observer = observer;
            try
            {
                this.observer.OnNext(value);
                this.observer.OnCompleted();
            }
            catch (Exception e)
            {
                this.observer.OnError(e);
            }
            return this;
        }

        public void Dispose()
        {
            (observer as IDisposable)?.Dispose();
        }
    }

    public static partial class ReactiveExtension
    {
        public static Observable<T> ToObservable<T>(this T value)
        {
            return new Observable<T>(value);
        }
    }
}