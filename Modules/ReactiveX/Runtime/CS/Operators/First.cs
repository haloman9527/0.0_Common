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
    public class First<T> : Operator<T>
    {
        bool published = true;

        public First(IObservable<T> src) : base(src) { }

        public override void OnNext(T value)
        {
            if (published)
            {
                published = false;
                base.OnNext(value);
            }
        }
    }

    public static partial class Extension
    {
        public static IObservable<T> First<T>(this IObservable<T> src)
        {
            return new First<T>(src);
        }
    }
}
