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
            return typeof(IFixedUpdateSystem);
        }

        public void Execute(Entity o)
        {
            Destroy((T)o);
        }

        protected abstract void Destroy(T o);
    }
}