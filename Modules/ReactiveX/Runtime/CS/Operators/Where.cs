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

        public Where(IObservable<IEnumerable<T>> _src, Func<T, bool> _filter) : base(_src)
        {
            filter = _filter;
        }

        public override void OnNext(IEnumerable<T> value)
        {
            observer.OnNext(Collection(value));

            IEnumerable<T> Collection(IEnumerable<T> _value)
            {
                foreach (var item in _value)
                {
                    if (filter(item))
                        yield return item;
                }
            }
        }
    }
}