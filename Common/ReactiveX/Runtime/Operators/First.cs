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
using System.Linq;

namespace CZToolKit.ReactiveX
{
    public class First<T> : Operator<IEnumerable<T>, T>
    {
        public First(IObservable<IEnumerable<T>> _src) : base(_src) { }

        public override void OnNext(IEnumerable<T> _value)
        {
            observer.OnNext(_value.First());
        }
    }

    public static partial class Extension
    {

        public static IObservable<T> First<T>(this IObservable<IEnumerable<T>> _src)
        {
            return new First<T>(_src);
        }
    }
}