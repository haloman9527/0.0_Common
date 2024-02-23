using System;

namespace CZToolKit
{
    public interface IAddComponentSystem : ISystem
    {
        void Execute(Entity o, Entity c);
    }

    public abstract class AddComponentSystem<T> : IAddComponentSystem where T : Entity
    {
        public Type EntityType()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IAddComponentSystem);
        }

        public void Execute(Entity o, Entity c)
        {
            AddComponent((T)o, c);
        }

        protected abstract void AddComponent(T self, Entity component);
    }
}