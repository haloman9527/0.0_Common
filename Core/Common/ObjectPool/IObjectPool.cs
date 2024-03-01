using System;

namespace CZToolKit
{
    public interface IObjectPool : IDisposable
    {
        int UnusedCount { get; }

        object Spawn();

        void Recycle(object unit);
    }
    
    public interface IObjectPool<T> : IObjectPool
    {
        void Recycle(T unit);
    }
}