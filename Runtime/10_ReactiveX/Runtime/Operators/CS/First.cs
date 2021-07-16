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

        public First(IObservable<T> _src) : base(_src) { }

        public override void OnNext(T _value)
        {
            if (published)
            {
                published = false;
                base.OnNext(_value);
            }
        }
    }
}
