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
    public class Execute<T> : Operator<T, T>
    {
        Action<T> func;
        public Execute(IObservable<T> src, Action<T> func) : base(src)
        {
            this.func = func;
        }

        public override void OnNext(T value)
        {
            func.Invoke(value);
            observer.OnNext(value);
        }
    }

    public static partial class Extension
    {
        public static IObservable<T> Execute<T>(this IObservable<T> src, Action<T> action)
        {
            return new Execute<T>(src, action);
        }
    }
}
