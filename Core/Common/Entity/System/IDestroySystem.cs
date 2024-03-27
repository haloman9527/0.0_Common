using System;

namespace CZToolKit
{
    public interface IDestroySystem : ISystem
    {
        void Execute(Entity o);
    }

    public abstract class DestroySystem<T> : IDestroySystem where T : Entity
    {
        public Type NodeType()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IDestroySystem);
        }

        public void Execute(Entity o)
        {
            Destroy((T)o);
        }

        protected abstract void Destroy(T o);
    }
}