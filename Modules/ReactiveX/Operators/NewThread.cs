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
 *  Github: https://github.com/haloman9527
 *  Blog: https://www.haloman.net/
 *
 */

#endregion

using System;
using System.Threading;

namespace CZToolKit.RX
{
    // 在一个新Task中执行接下来的任务
    public class NewThread<T> : Operator<T>
    {
        Thread thread;

        public NewThread(IObservable<T> src) : base(src)
        {
        }

        public override void OnNext(T value)
        {
            thread = new Thread(() => { Next(value); });
            thread.Start();
        }

        public override void OnDispose()
        {
            thread.Abort();
        }
    }

    public static partial class ReactiveExtension
    {
        public static IObservable<T> NewThread<T>(this IObservable<T> src)
        {
            return new NewThread<T>(src);
        }
    }
}