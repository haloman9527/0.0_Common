using System;

namespace Moyo
{
    public abstract class BaseEventArg : IPoolableObject, IDisposable
    {
        public abstract void OnSpawn();

        public abstract void OnRecycle();

        public virtual void Dispose()
        {
            ObjectPools.Recycle(this);
        }
    }

    public sealed class ValueEventArg<T> : BaseEventArg
    {
        public T value;
        
        public override void OnSpawn()
        {
            
        }

        public override void OnRecycle()
        {
            value = default;
        }
    }

    public sealed class EmptyEventArg : BaseEventArg
    {
        public static readonly EmptyEventArg Instance = new EmptyEventArg();

        public override void OnSpawn()
        {
        }

        public override void OnRecycle()
        {
        }

        public override void Dispose()
        {
        }
    }
}