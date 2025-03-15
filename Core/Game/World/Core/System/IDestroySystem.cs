using System;

namespace Atom
{
    public interface IDestroySystem : ISystem_E
    {
    }

    public abstract class DestroySystem<T> : IDestroySystem where T : Node
    {
        public Type NodeType() => TypeCache<T>.TYPE;

        public Type SystemType() => typeof(IDestroySystem);

        public void Execute(Node o) => Destroy((T)o);

        protected abstract void Destroy(T o);
    }
}