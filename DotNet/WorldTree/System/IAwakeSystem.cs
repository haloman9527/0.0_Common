using System;

namespace Moyo
{
    public interface IAwakeSystem : ISystem
    {
        void Execute(Node o);
    }

    public interface IAwakeSystem<A> : ISystem
    {
        void Execute(Node o, A a);
    }

    public interface IAwakeSystem<A, B> : ISystem
    {
        void Execute(Node o, A a, B b);
    }

    public interface IAwakeSystem<A, B, C> : ISystem
    {
        void Execute(Node o, A a, B b, C c);
    }

    public interface IAwakeSystem<A, B, C, D> : ISystem
    {
        void Execute(Node o, A a, B b, C c, D d);
    }

    public abstract class AwakeSystem<T> : IAwakeSystem where T : Node
    {
        public Type NodeType()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IAwakeSystem);
        }

        public void Execute(Node o)
        {
            Awake((T)o);
        }

        protected abstract void Awake(T o);
    }

    public abstract class AwakeSystem<T, A> : IAwakeSystem<A> where T : Node
    {
        public Type NodeType()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IAwakeSystem<A>);
        }

        public void Execute(Node o, A a)
        {
            Awake((T)o, a);
        }

        protected abstract void Awake(T o, A a);
    }

    public abstract class AwakeSystem<T, A, B> : IAwakeSystem<A, B> where T : Node
    {
        public Type NodeType()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IAwakeSystem<A, B>);
        }

        public void Execute(Node o, A a, B b)
        {
            Awake((T)o, a, b);
        }

        protected abstract void Awake(T o, A a, B b);
    }

    public abstract class AwakeSystem<T, A, B, C> : IAwakeSystem<A, B, C> where T : Node
    {
        public Type NodeType()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IAwakeSystem<A, B, C>);
        }

        public void Execute(Node o, A a, B b, C c)
        {
            Awake((T)o, a, b, c);
        }

        protected abstract void Awake(T o, A a, B b, C c);
    }
    
    public abstract class AwakeSystem<T, A, B, C, D> : IAwakeSystem<A, B, C, D> where T : Node
    {
        public Type NodeType()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IAwakeSystem<A, B, C, D>);
        }

        public void Execute(Node o, A a, B b, C c, D d)
        {
            Awake((T)o, a, b, c, d);
        }

        protected abstract void Awake(T o, A a, B b, C c, D d);
    }
}