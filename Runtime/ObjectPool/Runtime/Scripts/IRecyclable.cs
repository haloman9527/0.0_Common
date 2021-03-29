
namespace CZToolKit.Core.ObjectPool
{
    public interface IRecyclable
    {
        void OnSpawn();
        void OnRecycle();
    }
}
