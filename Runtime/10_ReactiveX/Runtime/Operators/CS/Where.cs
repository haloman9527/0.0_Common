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
    public class Where<T> : Operator<T>
    {
        Func<T, bool> filter;

        public Where(IObservable<T> _src, Func<T, bool> _filter) : base(_src)
        {
            filter = _filter;
        }

        public override void OnNext(T _value)
        {
            if (filter(_value))
                base.OnNext(_value);
        }
    }
}