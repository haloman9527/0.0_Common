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
using UnityEditor;

namespace CZToolKit.Core.Editors
{
    public interface ICondition
    {
        bool Result(EditorCoroutine _coroutine);
    }

    public class EditorWaitForSeconds : ICondition
    {
        readonly float seconds;

        public EditorWaitForSeconds(float _seconds) { seconds = _seconds; }

        public bool Result(EditorCoroutine _coroutine)
        {
            return EditorApplication.timeSinceStartup >= _coroutine.TimeSinceStartup + seconds;
        }
    }

    public class EditorWaitUntil : ICondition
    {
        readonly Func<bool> predicate;

        public EditorWaitUntil(Func<bool> _predicate) { predicate = _predicate; }

        public bool Result(EditorCoroutine _coroutine)
        {
            return predicate();
        }
    }
}