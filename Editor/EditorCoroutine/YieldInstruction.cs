using System;
using UnityEditor;

namespace CZToolKit.Core.Editors
{
    public interface ICondition
    {
        bool Result(EditorCoroutine _coroutine);
    }

    public class WaitForSeconds_E : ICondition
    {
        readonly float interval;

        public WaitForSeconds_E(float _interval) { interval = _interval; }

        public bool Result(EditorCoroutine _coroutine)
        {
            return EditorApplication.timeSinceStartup >= _coroutine.TimeSinceStartup + interval;
        }
    }

    public class WaitUntil_E : ICondition
    {
        readonly Func<bool> predicate;

        public WaitUntil_E(Func<bool> _predicate) { predicate = _predicate; }

        public bool Result(EditorCoroutine _coroutine)
        {
            return predicate();
        }
    }
}