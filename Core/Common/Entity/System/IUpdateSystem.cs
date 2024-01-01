using System;

namespace CZToolKit.ET
{
    public interface IUpdate
    {
    }

    public interface IUpdateSystem : ISystem
    {
        void Execute(Entity o);
    }

    public abstract class UpdateSystem<T> : IUpdateSystem where T : Entity
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
            Destroy((T)o);
        }

        protected abstract void Destroy(T o);
    }
}