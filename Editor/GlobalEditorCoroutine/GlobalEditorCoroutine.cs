using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace CZToolKit.Core.Editors
{
    public class GlobalEditorCoroutine
    {
        static GlobalEditorCoroutine()
        {
            EditorApplication.update += Update;
        }

        static Stack<EditorCoroutine> coroutineStack = new Stack<EditorCoroutine>();

        static void Update()
        {
            int count = coroutineStack.Count;
            while (count-- > 0)
            {
                EditorCoroutine coroutine = coroutineStack.Pop();
                if (!coroutine.IsRunning) continue;
                ICondition condition = coroutine.Current as ICondition;
                if (condition == null || condition.Result(coroutine))
                {
                    if (!coroutine.MoveNext())
                        continue;
                }
                coroutineStack.Push(coroutine);
            }
        }

        public static EditorCoroutine StartCoroutine(IEnumerator _coroutine)
        {
            EditorCoroutine coroutine = new EditorCoroutine(_coroutine);
            coroutineStack.Push(coroutine);
            return coroutine;
        }

        public static void StopCoroutine(EditorCoroutine _coroutine)
        {
            _coroutine.Stop();
        }
    }
}
