using System;

namespace CZToolKit
{
    public interface IUpdateSystem : ISystem
    {
        void Execute(Node o);
    }

    public abstract class UpdateSystem<T> : IUpdateSystem where T : Node
    {
        public Type NodeType()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IUpdateSystem);
        }

        public void Execute(Node o)
        {
            Update((T)o);
        }

        protected abstract void Update(T o);
    }
}