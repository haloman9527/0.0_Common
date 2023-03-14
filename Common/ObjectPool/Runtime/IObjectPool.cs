namespace CZToolKit.Common.ObjectPool
{
    public interface IObjectPool<T> where T : class
    {
        int UnusedCount { get; }
        
        T Spawn();
        void Recycle(T unit);
        void Release();
    }
}