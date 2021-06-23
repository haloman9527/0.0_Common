
namespace CZToolKit.Core.ObjectPool
{
    public interface IPoolable
    {
        void OnSpawned();

        void OnRecycled();
    }
}
