using System;
using System.Collections.Generic;
using System.Reflection;

namespace Atom
{
    public class ObjectPoolAttribute : Attribute
    {
        public readonly Type unitType;

        public ObjectPoolAttribute(Type unitType)
        {
            this.unitType = unitType;
        }
    }
    
    public static partial class ObjectPoolService
    {
        private class ObjectPool<T> : Atom.ObjectPool<T> where T : class, new()
        {
            protected override T Create()
            {
                return new T();
            }
        }
    }

    public static partial class ObjectPoolService
    {
        private static bool s_Initialized;
        private static Dictionary<int, IObjectPool> s_Pools = new Dictionary<int, IObjectPool>(64);

        static ObjectPoolService()
        {
            Init();
        }
        
        public static void Init(bool force = false)
        {
            if (s_Initialized && !force)
            {
                return;
            }

            s_Pools.Clear();

            var baseType = typeof(IObjectPool);
            foreach (var type in TypesCache.AllTypes)
            {
                if (!baseType.IsAssignableFrom(type))
                {
                    continue;
                }

                var attribute = type.GetCustomAttribute<ObjectPoolAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                var pool = Activator.CreateInstance(type);
                s_Pools.Add(attribute.unitType.GetHashCode(), pool as IObjectPool);
            }
        }

        private static IObjectPool GetOrCreatePool(Type unitType)
        {
            var hash = unitType.GetHashCode();
            if (!s_Pools.TryGetValue(hash, out var pool))
            {
                var poolType = typeof(ObjectPool<>).MakeGenericType(unitType);
                s_Pools[hash] = pool = (IObjectPool)Activator.CreateInstance(poolType);
            }

            return pool;
        }

        private static IObjectPool<T> GetOrCreatePool<T>() where T : class, new()
        {
            var unitType = TypeCache<T>.TYPE;
            var hash = unitType.GetHashCode();
            if (!s_Pools.TryGetValue(hash, out var pool))
            {
                s_Pools[hash] = pool = new ObjectPool<T>();
            }

            return (IObjectPool<T>)pool;
        }

        public static IObjectPool GetPool(Type unitType)
        {
            s_Pools.TryGetValue(unitType.GetHashCode(), out var pool);
            return pool;
        }

        public static void RegisterPool(IObjectPool pool)
        {
            s_Pools.Add(pool.UnitType.GetHashCode(), pool);
        }

        public static void ReleasePool(Type unitType)
        {
            var pool = GetPool(unitType);
            if (pool == null)
                return;

            pool.Dispose();
            s_Pools.Remove(unitType.GetHashCode());
        }

        public static T Spawn<T>() where T : class, new()
        {
            var unit = GetOrCreatePool<T>().Spawn();
            if (unit is IPoolableObject poolableObject)
            {
                poolableObject.OnSpawn();
            }

            return unit;
        }

        public static object Spawn(Type unitType)
        {
            var pool = GetOrCreatePool(unitType);
            if (pool == null)
            {
                return null;
            }
            
            var unit = pool.Spawn();
            if (unit is IPoolableObject poolableObject)
            {
                poolableObject.OnSpawn();
            }

            return unit;
        }

        public static void Recycle(Type unitType, object unit)
        {
            var pool = GetPool(unitType);
            if (pool == null)
                return;

            if (unit is IPoolableObject poolableObject)
            {
                poolableObject.OnRecycle();
            }

            pool.Recycle(unit);
        }

        public static void Recycle(object unit)
        {
            Recycle(unit.GetType(), unit);
        }
    }
}