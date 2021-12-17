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
using System;
using UnityEditor;

namespace CZToolKit.Core.Editors
{
    public class EditorWaitForSeconds : ICondition
    {
        readonly float seconds;

        public EditorWaitForSeconds(float seconds)
        {
            this.seconds = seconds;
        }

        public bool Result(EditorCoroutine coroutine)
        {
            return EditorApplication.timeSinceStartup >= coroutine.TimeSinceStartup + seconds;
        }
    }

    public class EditorWaitUntil : ICondition
    {
        readonly Func<bool> predicate;

        public EditorWaitUntil(Func<bool> predicate)
        {
            this.predicate = predicate;
        }

        public bool Result(EditorCoroutine coroutine)
        {
            return predicate();
        }
    }
}
#endif