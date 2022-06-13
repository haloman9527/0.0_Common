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

namespace CZToolKit.Core.ReactiveX
{
    public class Where<T> : Operator<IEnumerable<T>, IEnumerable<T>>
    {
        Func<T, bool> filter;

        public Where(IObservable<IEnumerable<T>> src, Func<T, bool> filter) : base(src)
        {
            this.filter = filter;
        }

        public override void OnNext(IEnumerable<T> value)
        {
            observer.OnNext(Collection());

            IEnumerable<T> Collection()
            {
                foreach (var item in value)
                {
                    if (filter(item))
                        yield return item;
                }
            }
        }
    }

    public static partial class Extension
    {
        /// <summary> 当src满足某条件 </summary>
        public static IObservable<IEnumerable<T>> Where<T>(this IObservable<IEnumerable<T>> src, Func<T, bool> filter)
        {
            return new Where<T>(src, filter);
        }
    }
}