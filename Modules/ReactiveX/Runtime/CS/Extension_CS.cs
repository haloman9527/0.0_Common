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
using System.Collections.Generic;

namespace CZToolKit.Core.ReactiveX
{
    public static partial class Extension
    {
        /// <summary> 当src满足某条件 </summary>
        public static IObservable<IEnumerable<T>> Where<T>(this IObservable<IEnumerable<T>> _src, Func<T, bool> _filter)
        {
            return new Where<T>(_src, _filter);
        }

        public static IObservable<IEnumerable<TOut>> Select<TIn, TOut>(this IObservable<IEnumerable<TIn>> _src, Func<TIn, TOut> _filter)
        {
            return new Select<TIn, TOut>(_src, _filter);
        }

        public static IObservable<T> Foreach<T>(this IObservable<IEnumerable<T>> _src)
        {
            return new Foreach<IEnumerable<T>, T>(_src);
        }

        public static IObservable<IEnumerable<T>> Foreach<T>(this IObservable<IEnumerable<T>> _src, Action<T> _action)
        {
            return new Foreach<T>(_src, _action);
        }

        public static IObservable<T> First<T>(this IObservable<T> _src)
        {
            return new First<T>(_src);
        }

        public static IObservable<T> Execute<T>(this IObservable<T> _src, Action<T> _action)
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
    }
}