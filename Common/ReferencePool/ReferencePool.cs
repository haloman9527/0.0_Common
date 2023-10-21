using System;
using System.Collections.Generic;

namespace CZToolKit
{
    public static class ReferencePool
    {
        private static Dictionary<Type, Pool> s_Pools = new Dictionary<Type, Pool>();

        private static Pool GetPool(Type unitType)
        {
            if (!s_Pools.TryGetValue(unitType, out var pool))
                s_Pools[unitType] = pool = new Pool();
            return pool;
        }
        
        public static T Acquire<T>() where T : class, IReference, new()
        {
            return GetPool(typeof(T)).Acquire<T>();
        }

        public static void Release(IReference reference)
        {
            var unitType = reference.GetType(); 
            GetPool(unitType).Release(reference);
        }
        
        public class Pool
        {
            private Queue<IReference> unusedObjects = new Queue<IReference>();

            public int UnusedCount
            {
                get { return unusedObjects.Count; }
            }

            /// <summary> 生成 </summary>
            public T Acquire<T>() where T : class, IReference, new()
            {
                T unit = null;
                if (unusedObjects.Count > 0)
                    unit = unusedObjects.Dequeue() as T;
                else
                    unit = new T();
                return (T)unit;
            }

            public void Release(IReference unit)
            {
                unit.Clear();
                unusedObjects.Enqueue(unit);
            }

            public void Dispose()
            {
                while (unusedObjects.Count > 0)
                {
                    unusedObjects.Dequeue().Clear();
                }
            }
        }
    }
}