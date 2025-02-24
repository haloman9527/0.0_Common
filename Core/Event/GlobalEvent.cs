using System;
using Moyo.Internal;

namespace Moyo
{
    public abstract class GlobalEvent<E> : IGlobalEvent
    {
        public Type EventType => TypeCache<E>.TYPE;

        public abstract void Invoke(E arg);
    }
}