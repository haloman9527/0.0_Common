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

namespace CZToolKit.Core.ReactiveX
{
    public class Select<TIn, TOut> : Operator<TIn, TOut>
    {
        Func<TIn, TOut> selector;

        public Select(IObservable<TIn> _src, Func<TIn, TOut> _selector) : base(_src)
        {
            selector = _selector;
        }

        public override void OnNext(TIn _value)
        {
            observer.OnNext(selector(_value));
        }
    }
}
