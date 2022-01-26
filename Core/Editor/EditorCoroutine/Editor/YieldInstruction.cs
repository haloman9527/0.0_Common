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
    public class EditorWaitForSeconds : IYield
    {
        readonly float seconds;

        public EditorWaitForSeconds(float seconds)
        {
            this.seconds = seconds;
        }

        public bool Result(ICoroutine coroutine)
        {
            var editorCoroutine = coroutine as EditorCoroutine;
            return EditorApplication.timeSinceStartup >= editorCoroutine.TimeSinceStartup + seconds;
        }
    }

    public class EditorWaitUntil : IYield
    {
        readonly Func<bool> predicate;

        public EditorWaitUntil(Func<bool> predicate)
        {
            this.predicate = predicate;
        }

        public bool Result(ICoroutine coroutine)
        {
            return predicate();
        }
    }
}
#endif