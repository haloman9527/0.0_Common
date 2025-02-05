using System;

namespace Moyo
{
    public interface IAwakeSystem : ISystem_E
    {
    }

    public interface IAwakeSystem<T0> : ISystem_EA<T0>
    {
    }

    public interface IAwakeSystem<T0, T1> : ISystem_EAA<T0, T1>
    {
    }

    public interface IAwakeSystem<T0, T1, T2> : ISystem_EAAA<T0, T1, T2>
    {
    }

    public interface IAwakeSystem<T0, T1, T2, T3> : ISystem_EAAAA<T0, T1, T2, T3>
    {
    }

    public abstract class AwakeSystem<N> : IAwakeSystem where N : Node
    {
        public Type NodeType() => TypeCache<N>.TYPE;

        public Type SystemType() => TypeCache<IAwakeSystem>.TYPE;

        public void Execute(Node o) => Awake((N)o);

        protected abstract void Awake(N o);
    }

    public abstract class AwakeSystem<N, T0> : IAwakeSystem<T0> where N : Node
    {
        public Type NodeType() => TypeCache<N>.TYPE;

        public Type SystemType() => TypeCache<IAwakeSystem<T0>>.TYPE;

        public void Execute(Node o, T0 arg0) => Awake((N)o, arg0);

        protected abstract void Awake(N o, T0 arg0);
    }

    public abstract class AwakeSystem<N, T0, T1> : IAwakeSystem<T0, T1> where N : Node
    {
        public Type NodeType() => TypeCache<N>.TYPE;

        public Type SystemType() => TypeCache<IAwakeSystem<T0, T1>>.TYPE;

        public void Execute(Node o, T0 arg0, T1 arg1) => Awake((N)o, arg0, arg1);

        protected abstract void Awake(N o, T0 arg0, T1 arg1);
    }

    public abstract class AwakeSystem<N, T0, T1, T2> : IAwakeSystem<T0, T1, T2> where N : Node
    {
        public Type NodeType() => TypeCache<N>.TYPE;

        public Type SystemType() => TypeCache<IAwakeSystem<T0, T1, T2>>.TYPE;

        public void Execute(Node o, T0 arg0, T1 arg1, T2 arg2) => Awake((N)o, arg0, arg1, arg2);

        protected abstract void Awake(N o, T0 arg0, T1 arg1, T2 arg2);
    }

    public abstract class AwakeSystem<N, T0, T1, T2, T3> : IAwakeSystem<T0, T1, T2, T3> where N : Node
    {
        public Type NodeType() => TypeCache<N>.TYPE;

        public Type SystemType() => TypeCache<IAwakeSystem<T0, T1, T2, T3>>.TYPE;

        public void Execute(Node o, T0 arg0, T1 arg1, T2 arg2, T3 arg3) => Awake((N)o, arg0, arg1, arg2, arg3);

        protected abstract void Awake(N o, T0 arg0, T1 arg1, T2 arg2, T3 arg3);
    }
}