using System;

namespace CZToolKit
{
    public interface IFixedUpdateSystem : ISystem
    {
        void Execute(Entity o);
    }

    public abstract class FixedUpdateSystem<T> : IFixedUpdateSystem where T : Entity
    {
        public Type NodeType()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IFixedUpdateSystem);
        }

        public void Execute(Entity o)
        {
            FixedUpdate((T)o);
        }

        protected abstract void FixedUpdate(T o);
    }
}