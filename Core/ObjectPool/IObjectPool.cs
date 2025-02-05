using System;

namespace Moyo
{
    public interface IObjectPool : IDisposable
    {
        Type UnitType { get; }
        
        int UnusedCount { get; }

        object Spawn();

        void Recycle(object unit);
    }
    
    public interface IObjectPool<T> : IDisposable where T : class
    {
        Type UnitType { get; }
        
        int UnusedCount { get; }

        T Spawn();
        
        void Recycle(T unit);
    }
}