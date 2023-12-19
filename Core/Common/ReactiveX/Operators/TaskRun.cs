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
 *  Blog: https://www.mindgear.net/
 *
 */

#endregion

using System;
using System.Threading.Tasks;

namespace CZToolKit.RX
{
    // 在一个新Task中执行接下来的任务
    public class TaskRun<T> : Operator<T>
    {
        Task task;

        public TaskRun(IObservable<T> _src) : base(_src)
        {
        }

        public override void OnNext(T value)
        {
            task = Task.Run(() => { Next(value); });
        }

        public override void OnDispose()
        {
            task.Dispose();
        }
    }

    public static partial class ReactiveExtension
    {
        public static IObservable<T> TaskRun<T>(this IObservable<T> src)
        {
            return new TaskRun<T>(src);
        }
    }
}