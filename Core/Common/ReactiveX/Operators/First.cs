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
 *  Blog: https://www.mindgear.net/
 *
 */
#endregion
using System;
using System.Collections.Generic;
using System.Linq;

namespace CZToolKit.RX
{
    public class First<T> : Operator<IEnumerable<T>, T>
    {
        public First(IObservable<IEnumerable<T>> _src) : base(_src) { }

        public override void OnNext(IEnumerable<T> _value)
        {
            Next(_value.First());
        }
    }

    public static partial class ReactiveExtension
    {

        public static IObservable<T> First<T>(this IObservable<IEnumerable<T>> _src)
        {
            return new First<T>(_src);
        }
    }
}