using System;

namespace CZToolKit.ET
{
    public interface IParentChanged
    {
    }

    public interface IParentChangedSystem : ISystem
    {
        void Execute(Entity o, Entity oldParent);
    }

    public abstract class ParentChangedSystem<T> : IParentChangedSystem where T : Entity
    {
        public Type Type()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IParentChangedSystem);
        }

        public void Execute(Entity o, Entity oldParent)
        {
            ParentChanged((T)o, oldParent);
        }

        protected abstract void ParentChanged(T o, Entity oldParent);
    }
}