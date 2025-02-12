using System;

namespace Moyo
{
    public abstract class EventArg : IPoolableObject, IDisposable
    {
        public abstract void OnSpawn();

        public abstract void OnRecycle();

        public void Dispose()
        {
            ObjectPoolService.Recycle(this);
        }
    }

    public struct ValueEventArg<T>
    {
        public T value;
    }

    public sealed class EmptyEventArg : EventArg
    {
        public static readonly EmptyEventArg Instance = new EmptyEventArg();

        public override void OnSpawn()
        {
        }

        public override void OnRecycle()
        {
        }
    }
}