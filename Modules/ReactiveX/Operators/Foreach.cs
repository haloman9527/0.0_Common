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
using System.Collections.Generic;

namespace CZToolKit.RX
{
    public class Foreach<TIn, TOut> : Operator<TIn, TOut> where TIn : IEnumerable<TOut>
    {
        public Foreach(IObservable<TIn> src) : base(src) { }

        public override void OnNext(TIn value)
        {
            foreach (TOut v in value)
            {
                Next(v);
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

            Next(value);
        }
    }

    public static partial class ReactiveExtension
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
