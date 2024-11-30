using System;

namespace Moyo
{
    public interface IDestroySystem : ISystem
    {
        void Execute(Node o);
    }

    public abstract class DestroySystem<T> : IDestroySystem where T : Node
    {
        public Type NodeType()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IDestroySystem);
        }

        public void Execute(Node o)
        {
            Destroy((T)o);
        }

        protected abstract void Destroy(T o);
    }
}