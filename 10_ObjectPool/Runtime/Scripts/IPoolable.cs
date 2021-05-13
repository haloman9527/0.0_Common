
namespace CZToolKit.Core.ObjectPool
{
    public interface IPoolable
    {
        void OnSpawn();
        void OnRecycle();
    }
}
