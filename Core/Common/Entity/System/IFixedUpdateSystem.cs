using System;

namespace CZToolKit.ET
{
    public interface IFixedUpdate
    {
    }

    public interface IFixedUpdateSystem : ISystem
    {
        void Execute(Entity o);
    }

    public abstract class FixedUpdateSystem<T> : IFixedUpdateSystem where T : Entity
    {
        public Type Type()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IUpdateSystem);
        }

        public void Execute(Entity o)
        {
            FixedUpdate((T)o);
        }

        protected abstract void FixedUpdate(T o);
    }
}