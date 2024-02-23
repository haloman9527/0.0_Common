using System;

namespace CZToolKit
{
    public interface IOnCreateSystem : ISystem
    {
        void Execute(Entity o);
    }

    public abstract class OnCreateSystem<T> : IOnCreateSystem where T : Entity
    {
        public Type EntityType()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IOnCreateSystem);
        }

        public void Execute(Entity o)
        {
            OnCreate((T)o);
        }

        protected abstract void OnCreate(T o);
    }
}