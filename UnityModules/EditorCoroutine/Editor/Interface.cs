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

namespace CZToolKit.EditorCoroutine
{
    public interface IYield
    {
        bool Result(ICoroutine coroutine);
    }

    public interface ICoroutine
    {
        bool IsRunning { get; }
        IYield Current { get; }

        bool MoveNext();
        void Stop();
    }
}