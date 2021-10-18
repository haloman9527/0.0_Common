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
        public Foreach(IObservable<TIn> _src) : base(_src) { }

        public override void OnNext(TIn _value)
        {
            foreach (TOut value in _value)
            {
                observer.OnNext(value);
            }
        }
    }

    public class Foreach<TIn> : Operator<IEnumerable<TIn>>
    {
        Action<TIn> action;

        public Foreach(IObservable<IEnumerable<TIn>> _src, Action<TIn> _action) : base(_src)
        {
            action = _action;
        }

        public override void OnNext(IEnumerable<TIn> _value)
        {
            foreach (TIn value in _value)
            {
                action(value);
            }
            observer.OnNext(_value);
        }
    }
}
