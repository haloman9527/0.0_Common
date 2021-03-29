using System;
using System.Collections;
using System.Collections.Generic;

namespace CZToolKit.Core.RX
{
    public static class ObservableExtensions
    {
        public static Observable<T> ToObservable<T>(this T _src)
        {
            return new Observable<T>(_src);
        }

        public static IDisposable Subscribe<T>(this IObservable<T> _src)
        {
            return _src.Subscribe(new Subscribe<T>());
        }

        public static IDisposable Subscribe<T>(this IObservable<T> _src, Action<T> _onNext)
        {
            return _src.Subscribe(new Subscribe<T>(_onNext));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> _src, Action<T> _onNext, Action<Exception> _onError)
        {
            return _src.Subscribe(new Subscribe<T>(_onNext, _onError));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> _src, Action<T> _onNext, Action _onCompleted)
        {
            return _src.Subscribe(new Subscribe<T>(_onNext, _onCompleted));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> _src, Action<T> _onNext, Action<Exception> _onError, Action _onCompleted)
        {
            return _src.Subscribe(new Subscribe<T>(_onNext, _onError, _onCompleted));
        }

        #region Operators
        /// <summary> 当src满足某条件 </summary>
        public static IObservable<T> Where<T>(this IObservable<T> _src, Func<T, bool> _filter)
        {
            return new Where<T>(_src, _filter);
        }

        public static IObservable<TOut> Select<TIn, TOut>(this IObservable<TIn> _src, Func<TIn, TOut> _filter)
        {
            return new Select<TIn, TOut>(_src, _filter);
        }

        public static IObservable<T> Foreach<T>(this IObservable<IEnumerable<T>> _src)
        {
            return new Foreach<IEnumerable<T>, T>(_src);
        }

        public static IObservable<T> First<T>(this IObservable<T> _src)
        {
            return new First<T>(_src);
        }

        public static IObservable<T> Execute<T>(this IObservable<T> _src, Action _action)
        {
            return new Execute<T>(_src, _action);
        }

        public static IObservable<T> TaskRun<T>(this IObservable<T> _src)
        {
            return new TaskRun<T>(_src);
        }

        public static IObservable<T> NewThread<T>(this IObservable<T> _src)
        {
            return new NewThread<T>(_src);
        }

        #endregion
    }
}
