using System;

namespace CZToolKit
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