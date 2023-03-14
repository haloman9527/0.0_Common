
namespace CZToolKit.Common
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