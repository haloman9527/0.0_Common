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

namespace CZToolKit.ReactiveX
{
    public class Observable<T> : IDisposable, IObservable<T>
    {
        private readonly T _value;
        private IObserver<T> observer;

        public Observable(T value)
        {
            _value = value;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            this.observer = observer;
            this.observer.OnNext(_value);
            this.observer.OnCompleted();
            return this;
        }

        public void Dispose()
        {
            (observer as IDisposable)?.Dispose();
        }
    }

    public static partial class Extension
    {
        public static Observable<T> ToObservable<T>(this T src)
        {
            return new Observable<T>(src);
        }
    }
}