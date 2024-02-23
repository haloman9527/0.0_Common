using System;

namespace CZToolKit
{
    public interface IAwakeSystem : ISystem
    {
        void Execute(Entity o);
    }

    public interface IAwakeSystem<A> : ISystem
    {
        void Execute(Entity o, A a);
    }

    public interface IAwakeSystem<A, B> : ISystem
    {
        void Execute(Entity o, A a, B b);
    }

    public interface IAwakeSystem<A, B, C> : ISystem
    {
        void Execute(Entity o, A a, B b, C c);
    }

    public interface IAwakeSystem<A, B, C, D> : ISystem
    {
        void Execute(Entity o, A a, B b, C c, D d);
    }

    public abstract class AwakeSystem<T> : IAwakeSystem where T : Entity
    {
        public Type EntityType()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IAwakeSystem);
        }

        public void Execute(Entity o)
        {
            Awake((T)o);
        }

        protected abstract void Awake(T o);
    }

    public abstract class AwakeSystem<T, A> : IAwakeSystem<A> where T : Entity
    {
        public Type EntityType()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IAwakeSystem<A>);
        }

        public void Execute(Entity o, A a)
        {
            Awake((T)o, a);
        }

        protected abstract void Awake(T o, A a);
    }

    public abstract class AwakeSystem<T, A, B> : IAwakeSystem<A, B> where T : Entity
    {
        public Type EntityType()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IAwakeSystem<A, B>);
        }

        public void Execute(Entity o, A a, B b)
        {
            Awake((T)o, a, b);
        }

        protected abstract void Awake(T o, A a, B b);
    }

    public abstract class AwakeSystem<T, A, B, C> : IAwakeSystem<A, B, C> where T : Entity
    {
        public Type EntityType()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IAwakeSystem<A, B, C>);
        }

        public void Execute(Entity o, A a, B b, C c)
        {
            Awake((T)o, a, b, c);
        }

        protected abstract void Awake(T o, A a, B b, C c);
    }
    
    public abstract class AwakeSystem<T, A, B, C, D> : IAwakeSystem<A, B, C, D> where T : Entity
    {
        public Type EntityType()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IAwakeSystem<A, B, C, D>);
        }

        public void Execute(Entity o, A a, B b, C c, D d)
        {
            Awake((T)o, a, b, c, d);
        }

        protected abstract void Awake(T o, A a, B b, C c, D d);
    }
}