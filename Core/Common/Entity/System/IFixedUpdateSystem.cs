using System;

namespace CZToolKit
{
    public interface IFixedUpdateSystem : ISystem
    {
        void Execute(Node o);
    }

    public abstract class FixedUpdateSystem<T> : IFixedUpdateSystem where T : Node
    {
        public Type NodeType()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IFixedUpdateSystem);
        }

        public void Execute(Node o)
        {
            FixedUpdate((T)o);
        }

        protected abstract void FixedUpdate(T o);
    }
}