using System;

namespace CZToolKit.ET
{
    public interface ILateUpdate
    {
    }


    public interface ILateUpdateSystem : ISystem
    {
        void Execute(Entity o);
    }

    public abstract class LateUpdateSystem<T> : ILateUpdateSystem where T : Entity
    {
        public Type Type()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(ILateUpdateSystem);
        }

        public void Execute(Entity o)
        {
            LateUpdate((T)o);
        }

        protected abstract void LateUpdate(T o);
    }
}