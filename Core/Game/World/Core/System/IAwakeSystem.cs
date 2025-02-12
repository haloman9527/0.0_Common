using System;

namespace Moyo
{
    public interface IAwakeSystem : ISystem_E
    {
    }

    public interface IAwakeSystem<TArg> : ISystem_EA<TArg>
    {
    }

    public abstract class AwakeSystem<N> : IAwakeSystem where N : Node
    {
        public Type NodeType() => TypeCache<N>.TYPE;

        public Type SystemType() => TypeCache<IAwakeSystem>.TYPE;

        public void Execute(Node o) => Awake((N)o);

        protected abstract void Awake(N o);
    }

    public abstract class AwakeSystem<N, TArg> : IAwakeSystem<TArg> where N : Node
    {
        public Type NodeType() => TypeCache<N>.TYPE;

        public Type SystemType() => TypeCache<IAwakeSystem<TArg>>.TYPE;

        public void Execute(Node o, TArg arg) => Awake((N)o, arg);

        protected abstract void Awake(N o, TArg arg);
    }
}