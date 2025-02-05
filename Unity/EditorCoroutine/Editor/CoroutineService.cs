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
using System.Collections;
using System.Collections.Generic;

namespace Moyo.UnityEditors.EditorCoroutine
{
    public abstract class CoroutineService<T> where T : class, ICoroutine
    {
        protected readonly Queue<T> coroutineQueue = new Queue<T>();

        public void Update()
        {
            int count = coroutineQueue.Count;
            while (count-- > 0)
            {
                T coroutine = coroutineQueue.Dequeue();
                if (!coroutine.IsRunning) continue;
                IYield condition = coroutine.Current;
                if (condition == null || condition.Result(coroutine))
                {
                    if (!coroutine.MoveNext())
                        continue;
                }
                coroutineQueue.Enqueue(coroutine);
            }
        }

        public abstract T StartCoroutine(IEnumerator enumerator);

        public abstract void StopCoroutine(T coroutine);
    }
}