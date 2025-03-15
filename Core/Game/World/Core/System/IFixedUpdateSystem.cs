using System;

namespace Atom
{
    public interface IFixedUpdateSystem : ISystem_E
    {
    }

    public abstract class FixedUpdateSystem<N> : IFixedUpdateSystem where N : Node
    {
        public Type NodeType() => TypeCache<N>.TYPE;

        public Type SystemType() => TypeCache<IFixedUpdateSystem>.TYPE;

        public void Execute(Node o) => FixedUpdate((N)o);

        protected abstract void FixedUpdate(N o);
    }
}