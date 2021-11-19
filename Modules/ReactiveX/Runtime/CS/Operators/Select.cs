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

        public Select(IObservable<IEnumerable<TIn>> _src, Func<TIn, TOut> _selector) : base(_src)
        {
            selector = _selector;
        }

        public override void OnNext(IEnumerable<TIn> value)
        {
            observer.OnNext(Collection(value));

            IEnumerable<TOut> Collection(IEnumerable<TIn> _value)
            {
                foreach (var item in _value)
                {
                    yield return selector(item);
                }
            }
        }
    }
}
