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
    public class Execute<T> : Operator<T>
    {
        Action func;
        public Execute(IObservable<T> _src, Action _func) : base(_src)
        {
            func = _func;
        }

        public override void OnNext(T _value)
        {
            func.Invoke();
            base.OnNext(_value);
        }
    }
}
