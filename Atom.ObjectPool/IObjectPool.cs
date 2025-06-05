using System;

namespace Atom
{
    public interface IObjectPool
    {
        Type UnitType { get; }
        
        int UnusedCount { get; }

        object Spawn();

        void Recycle(object unit);

        void Release();
    }
    
    public interface IObjectPool<T> where T : class
    {
        Type UnitType { get; }
        
        int UnusedCount { get; }

        T Spawn();
        
        void Recycle(T unit);

        void Release();
    }
}