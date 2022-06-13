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
    public class Until<T> : Operator<T>
    {
        Func<T, bool> until;

        public Until(IObservable<T> src, Func<T, bool> until) : base(src)
        {
            this.until = until;
        }

        public override void OnNext(T value)
        {
            if (until(value))
                base.OnNext(value);
        }
    }
}