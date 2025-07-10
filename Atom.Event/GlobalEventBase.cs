using System;

namespace Atom
{
    public abstract class GlobalEventBase : EventBase
    {
        public abstract Type EventType { get; }
    }

    public abstract class GlobalEventBase<T> : GlobalEventBase, IEvent<T>
    {
        public override Type EventType
        {
            get { return TypeCache<T>.TYPE; }
        }

        public abstract void Invoke(in T arg);
    }
}