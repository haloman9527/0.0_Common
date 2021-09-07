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
    public interface IOnDestory
    {
        event Action onDestroy;
    }

    public class OnDestroy<TIn, TOut> : Operator<TIn, TOut> where TIn : IOnDestory where TOut : class, TIn
    {
        Action action;

        public OnDestroy(IObservable<TIn> _src, Action _action) : base(_src)
        {
            action = _action;
        }

        public override void OnNext(TIn _onDestroy)
        {
            _onDestroy.onDestroy += action;
            observer.OnNext(_onDestroy as TOut);
        }

        public override void OnDispose() { }
    }
}
