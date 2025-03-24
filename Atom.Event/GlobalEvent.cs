using System;
using Atom.Internal;

namespace Atom
{
    public abstract class GlobalEvent<E> : IGlobalEvent
    {
        public Type EventType => TypeCache<E>.TYPE;

        public abstract void Invoke(E arg);
    }
}