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

        public OnDestroy(IObservable<TIn> src, Action action) : base(src)
        {
            this.action = action;
        }

        public override void OnNext(TIn onDestroy)
        {
            onDestroy.onDestroy += action;
            observer.OnNext(onDestroy as TOut);
        }

        public override void OnDispose() { }
    }

    public static partial class Extension
    {
        public static IObservable<TIn> OnDestroy<TIn>(this IObservable<TIn> src, Action action) where TIn : class, IOnDestory
        {
            return new OnDestroy<TIn, TIn>(src, action);
        }
    }
}
