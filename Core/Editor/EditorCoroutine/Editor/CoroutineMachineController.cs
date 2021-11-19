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
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;

namespace CZToolKit.Core.Editors
{
    public class CoroutineMachineController
    {
        Queue<EditorCoroutine> coroutineQueue = new Queue<EditorCoroutine>();

        public void Update()
        {
            int count = coroutineQueue.Count;
            while (count-- > 0)
            {
                EditorCoroutine coroutine = coroutineQueue.Dequeue();
                if (!coroutine.IsRunning) continue;
                ICondition condition = coroutine.Current as ICondition;
                if (condition == null || condition.Result(coroutine))
                {
                    if (!coroutine.MoveNext())
                        continue;
                }
                coroutineQueue.Enqueue(coroutine);
            }
        }

        public EditorCoroutine StartCoroutine(IEnumerator enumerator)
        {
            EditorCoroutine coroutine = new EditorCoroutine(enumerator);
            coroutineQueue.Enqueue(coroutine);
            return coroutine;
        }

        public void StopCoroutine(EditorCoroutine coroutine)
        {
            coroutine.Stop();
        }
    }
}
#endif