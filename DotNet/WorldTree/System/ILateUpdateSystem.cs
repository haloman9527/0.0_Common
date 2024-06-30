using System;

namespace CZToolKit
{
    public interface ILateUpdateSystem : ISystem
    {
        void Execute(Node o);
    }

    public abstract class LateUpdateSystem<T> : ILateUpdateSystem where T : Node
    {
        public Type NodeType()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(ILateUpdateSystem);
        }

        public void Execute(Node o)
        {
            LateUpdate((T)o);
        }

        protected abstract void LateUpdate(T o);
    }
}