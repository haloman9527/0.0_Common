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
using System.Collections.Generic;

namespace CZToolKit.Core.ReactiveX
{
    public class Foreach<TIn, TOut> : Operator<TIn, TOut> where TIn : IEnumerable<TOut>
    {
        public Foreach(IObservable<TIn> src) : base(src) { }

        public override void OnNext(TIn value)
        {
            foreach (TOut v in value)
            {
                observer.OnNext(v);
            }
        }
    }

    public class Foreach<TIn> : Operator<IEnumerable<TIn>>
    {
        Action<TIn> action;

        public Foreach(IObservable<IEnumerable<TIn>> _src, Action<TIn> action) : base(_src)
        {
            this.action = action;
        }

        public override void OnNext(IEnumerable<TIn> value)
        {
            foreach (TIn v in value)
            {
                action(v);
            }
            observer.OnNext(value);
        }
    }

    public static partial class Extension
    {
        public static IObservable<T> Foreach<T>(this IObservable<IEnumerable<T>> src)
        {
            return new Foreach<IEnumerable<T>, T>(src);
        }

        public static IObservable<IEnumerable<T>> Foreach<T>(this IObservable<IEnumerable<T>> src, Action<T> action)
        {
            return new Foreach<T>(src, action);
        }
    }
}
