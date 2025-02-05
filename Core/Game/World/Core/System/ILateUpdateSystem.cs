using System;

namespace Moyo
{
    public interface ILateUpdateSystem : ISystem_E
    {
    }

    public abstract class LateUpdateSystem<N> : ILateUpdateSystem where N : Node
    {
        public Type NodeType() => TypeCache<N>.TYPE;

        public Type SystemType() => TypeCache<ILateUpdateSystem>.TYPE;

        public void Execute(Node o) => LateUpdate((N)o);

        protected abstract void LateUpdate(N o);
    }
}