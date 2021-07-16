using System.Collections;
using UnityEditor;

namespace CZToolKit.Core.Editors
{
    public class EditorCoroutine : ICondition
    {
        IEnumerator enumerator;

        public bool IsRunning { get; private set; } = true;
        public double TimeSinceStartup { get; private set; }
        public object Current { get { return enumerator.Current; } }

        public EditorCoroutine(IEnumerator _enumerator)
        {
            enumerator = _enumerator;
        }

        public bool MoveNext()
        {
            TimeSinceStartup = EditorApplication.timeSinceStartup;
            return IsRunning = enumerator.MoveNext();
        }

        public void Stop() { IsRunning = false; }

        public bool Result(EditorCoroutine _coroutine)
        {
            return !IsRunning;
        }
    }
}