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
    public class Select<TIn, TOut> : Operator<IEnumerable<TIn>, IEnumerable<TOut>>
    {
        Func<TIn, TOut> selector;

        public Select(IObservable<IEnumerable<TIn>> src, Func<TIn, TOut> _selector) : base(src)
        {
            selector = _selector;
        }

        public override void OnNext(IEnumerable<TIn> value)
        {
            observer.OnNext(Collection());

            IEnumerable<TOut> Collection()
            {
                foreach (var v in value)
                {
                    yield return selector(v);
                }
            }
        }
    }

    public static partial class Extension
    {
        public static IObservable<IEnumerable<TOut>> Select<TIn, TOut>(this IObservable<IEnumerable<TIn>> src, Func<TIn, TOut> filter)
        {
            return new Select<TIn, TOut>(src, filter);
        }
    }
}
