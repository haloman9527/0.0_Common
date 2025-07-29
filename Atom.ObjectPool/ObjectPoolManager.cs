using System;
using System.Collections.Generic;

namespace Atom
{
    public static partial class ObjectPoolManager
    {
        private static Dictionary<int, IObjectPool> s_ObjectPools;

        static ObjectPoolManager()
        {
            s_ObjectPools = new Dictionary<int, IObjectPool>(64);
        }

        /// <summary>
        /// 注册一个对象池
        /// </summary>
        /// <param name="objectPool"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void RegisterPool(IObjectPool objectPool)
        {
            if (HasPool(objectPool.ObjectType))
                throw new InvalidOperationException($"already registered pool for type {objectPool.ObjectType}");
            
            s_ObjectPools.Add(objectPool.ObjectType.GetHashCode(), objectPool);
        }

        public static void ReleasePool(Type unitType)
        {
            var objectPool = GetPool(unitType);
            if (objectPool == null)
                throw new InvalidOperationException($"can not found pool for type {unitType}");
            
            s_ObjectPools.Remove(unitType.GetHashCode());
            objectPool.ReleaseAll();
        }

        public static bool HasPool(Type unitType)
        {
            return s_ObjectPools.ContainsKey(unitType.GetHashCode());
        }

        public static IObjectPool GetPool(Type unitType)
        {
            s_ObjectPools.TryGetValue(unitType.GetHashCode(), out var objectPool);
            return objectPool;
        }

        public static IObjectPool GetPool<T>()
        {
            s_ObjectPools.TryGetValue(TypeCache<T>.HASH, out var objectPool);
            return objectPool;
        }

        public static T Spawn<T>() where T : class, new()
        {
            var objectPool = GetPool<T>() as IObjectPool<T>;
            if (objectPool == null)
            {
                objectPool = new ObjectPool<T>();
                RegisterPool(objectPool);
            }
            
            var unit = objectPool.Spawn();
            if (unit is IObjectPoolable obj)
                obj.OnSpawn();
            
            return unit;
        }

        public static object Spawn(Type unitType)
        {
            var objectPool = GetPool(unitType);
            if (objectPool == null)
                throw new InvalidOperationException($"can not found pool for type {unitType}");

            var unit = objectPool.Spawn();
            if (unit is IObjectPoolable obj)
                obj.OnSpawn();

            return unit;
        }

        public static void Recycle(object unit)
        {
            Recycle(unit.GetType(), unit);
        }

        public static void Recycle(Type unitType, object unit)
        {
            var objectPool = GetPool(unitType);
            if (objectPool == null)
                throw new InvalidOperationException($"can not found pool for type {unitType}");

            if (unit is IObjectPoolable poolableObject)
                poolableObject.OnRecycle();

            objectPool.Recycle(unit);
        }
    }
}