using System;

namespace CZToolKit.ObjectPool
{
    public interface IObjectPool : IDisposable
    {
        int UnusedCount { get; }

        object Acquire();

        void Release(object unit);
    }
    
    public interface IObjectPool<T> : IObjectPool
    {
        T Acquire();
        
        void Release(T unit);
    }
}