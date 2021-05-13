using CZToolKit.Core.Singletons;

namespace CZToolKit.Core.ReactiveX
{
    public class MainThreadDispatcher : CZMonoSingleton<MainThreadDispatcher>
    {
        protected override void OnClean()
        {
            StopAllCoroutines();
        }
    }
}
