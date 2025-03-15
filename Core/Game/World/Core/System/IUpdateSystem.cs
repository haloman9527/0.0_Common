using System;

namespace Atom
{
    public interface IUpdateSystem : ISystem_E
    {
    }

    public abstract class UpdateSystem<N> : IUpdateSystem where N : Node
    {
        public Type NodeType() => TypeCache<N>.TYPE;

        public Type SystemType() => TypeCache<IUpdateSystem>.TYPE;

        public void Execute(Node o) => Update((N)o);

        protected abstract void Update(N o);
    }
}