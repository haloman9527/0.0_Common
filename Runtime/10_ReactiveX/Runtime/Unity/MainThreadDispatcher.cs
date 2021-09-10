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
using CZToolKit.Core.Singletons;

namespace CZToolKit.Core.ReactiveX
{
    public class MainThreadDispatcher : CZMonoSingleton<MainThreadDispatcher>
    {
        protected override void OnBeforeDestroy()
        {
            StopAllCoroutines();
        }
    }
}
