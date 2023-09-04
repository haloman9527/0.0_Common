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

namespace CZToolKit.RX
{
    public class OnTrigger<TIn> : Operator<UnityEvent<TIn>, TIn>
    {
        public OnTrigger(IObservable<UnityEvent<TIn>> src) : base(src) { }

        public override void OnNext(UnityEvent<TIn> value)
        {
            value.AddListener(v =>
            {
                Next(v);
            });
        }
    }
}
