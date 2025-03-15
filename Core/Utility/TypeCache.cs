using System;

namespace Atom
{
    public static class TypeCache<T>
    {
        public static readonly Type TYPE = typeof(T);
        
        public static readonly int HASH = typeof(T).GetHashCode();
    }
}