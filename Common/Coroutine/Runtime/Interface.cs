#region ◊¢  Õ
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: ∞Î÷ª¡˙œ∫»À
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */
#endregion

namespace CZToolKit.Core
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