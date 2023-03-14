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

namespace CZToolKit.Common
{
    public partial class EditorCoroutineService : CoroutineService<EditorCoroutine>
    {
        public override EditorCoroutine StartCoroutine(IEnumerator enumerator)
        {
            EditorCoroutine coroutine = new EditorCoroutine(enumerator);
            coroutineQueue.Enqueue(coroutine);
            return coroutine;
        }

        public override void StopCoroutine(EditorCoroutine coroutine)
        {
            coroutine.Stop();
        }
    }
}
#endif