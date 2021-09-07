#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 
 *
 */
#endregion
using System;
using UnityEngine.Events;

namespace CZToolKit.Core.ReactiveX
{
    public class OnTrigger<TIn> : Operator<UnityEvent<TIn>, TIn>
    {
        public OnTrigger(IObservable<UnityEvent<TIn>> _src) : base(_src) { }

        public override void OnNext(UnityEvent<TIn> _value)
        {
            _value.AddListener(v =>
            {
                observer.OnNext(v);
            });
        }
    }
}
