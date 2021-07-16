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
    public class Foreach<TIn, TOut> : Operator<TIn, TOut> where TIn : IEnumerable<TOut>
    {
        public Foreach(IObservable<TIn> _src) : base(_src) { }

        public override void OnNext(TIn _value)
        {
            foreach (TOut value in _value)
            {
                observer.OnNext(value);
            }
        }
    }
}
