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
        public Until(IObservable<T> _src, Func<T, bool> _until) : base(_src)
        {
            until = _until;
        }

        public override void OnNext(T _value)
        {
            if (until(_value))
                base.OnNext(_value);
        }
    }
}