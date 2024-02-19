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
 *  Github: https://github.com/haloman9527
 *  Blog: https://www.haloman.net/
 *
 */
#endregion
using System;

namespace CZToolKit.RX
{
    public class Execute<T> : Operator<T>
    {
        Action<T> func;
        public Execute(IObservable<T> src, Action<T> func) : base(src)
        {
            this.func = func;
        }

        public override void OnNext(T value)
        {
            func.Invoke(value);
            Next(value);
        }
    }

    public static partial class ReactiveExtension
    {
        public static IObservable<T> Execute<T>(this IObservable<T> src, Action<T> action)
        {
            return new Execute<T>(src, action);
        }
    }
}
