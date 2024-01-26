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
 *  Blog: https://www.mindgear.net/
 *
 */
#endregion
using System;

namespace CZToolKit.RX
{
    public interface IOperator : IDisposable { }

    public abstract class Operator<TIn> : IOperator, IObserver<TIn>, IObservable<TIn>
    {
        /// <summary>
        /// previous | in
        /// </summary>
        private IObservable<TIn> src;
        /// <summary>
        /// observer | out
        /// </summary>
        private IObserver<TIn> observer;

        public Operator(IObservable<TIn> src)
        {
            this.src = src;
        }

        protected void Next(TIn value)
        {
            observer.OnNext(value);
        }

        protected void NextError(Exception error)
        {
            observer.OnError(error);
        }

        protected void NextComplete()
        {
            observer.OnCompleted();
        }

        public abstract void OnNext(TIn value);

        public virtual void OnError(Exception error)
        {
            NextError(error);
        }

        public virtual void OnCompleted()
        {
            NextComplete();
        }

        public IDisposable Subscribe(IObserver<TIn> observer)
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

    public abstract class Operator<TIn, TOut> : IOperator, IObserver<TIn>, IObservable<TOut>
    {
        /// <summary>
        /// previous | in
        /// </summary>
        private IObservable<TIn> src;
        /// <summary>
        /// observer | out
        /// </summary>
        private IObserver<TOut> observer;

        public Operator(IObservable<TIn> src)
        {
            this.src = src;
        }

        protected void Next(TOut value)
        {
            observer.OnNext(value);
        }

        protected void NextError(Exception error)
        {
            observer.OnError(error);
        }

        protected void NextComplete()
        {
            observer.OnCompleted();
        }

        public void Dispose()
        {
            (observer as IDisposable)?.Dispose();
            OnDispose();
        }

        public abstract void OnNext(TIn value);

        public virtual void OnError(Exception error)
        {
            NextError(error);
        }

        public virtual void OnCompleted()
        {
            NextComplete();
        }

        public virtual IDisposable Subscribe(IObserver<TOut> observer)
        {
            this.observer = observer;
            return this.src.Subscribe(this);
        }

        public virtual void OnDispose() { }
    }
}