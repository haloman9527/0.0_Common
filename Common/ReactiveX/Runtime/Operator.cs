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
    public interface IOperator : IDisposable { }

    public abstract class Operator<T> : IOperator, IObservable<T>, IObserver<T>
    {
        /// <summary>
        /// previos
        /// </summary>
        protected IObservable<T> src;
        /// <summary>
        /// next
        /// </summary>
        protected IObserver<T> observer;

        public Operator(IObservable<T> src)
        {
            this.src = src;
        }

        public virtual void OnNext(T value)
        {
            observer.OnNext(value);
        }

        public virtual void OnError(Exception error)
        {
            observer.OnError(error);
        }

        public virtual void OnCompleted()
        {
            observer.OnCompleted();
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            this.observer = observer;
            return src.Subscribe(this);
        }

        public void Dispose()
        {
            (observer as IDisposable)?.Dispose();
            OnDispose();
        }

        public virtual void OnDispose() { }
    }

    public abstract class Operator<TIn, TOut> : IOperator, IObservable<TOut>, IObserver<TIn>
    {
        protected IObservable<TIn> src;
        protected IObserver<TOut> observer;


        public Operator(IObservable<TIn> src)
        {
            this.src = src;
        }

        public abstract void OnNext(TIn value);

        public virtual void OnError(Exception error)
        {
            observer.OnError(error);
        }

        public virtual void OnCompleted()
        {
            observer.OnCompleted();
        }

        public virtual IDisposable Subscribe(IObserver<TOut> observer)
        {
            this.observer = observer;
            return src.Subscribe(this);
        }

        public void Dispose()
        {
            (observer as IDisposable)?.Dispose();
            OnDispose();
        }

        public virtual void OnDispose() { }
    }
}