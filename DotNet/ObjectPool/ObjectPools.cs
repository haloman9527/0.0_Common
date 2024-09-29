#region 注 释

/***
 *
 *  Title: ""
 *      主题: 对象池基类，只有最基本的功能
 *  Description:
 *      功能:
 *      	1.生成一个对象
 *      	2.回收一个对象
 *  Date:
 *  Version:
 *  Writer: 半只龙虾人
 *  Github: https://github.com/haloman9527
 *  Blog: https://www.haloman.net/
 *
 */

#endregion

using System;
using System.Collections.Generic;
using System.Reflection;

namespace CZToolKit
{
    public class ObjectPool<T> : BaseObjectPool<T> where T : class, new()
    {
        protected override T Create()
        {
            return new T();
        }
    }

    public interface IPoolableObject
    {
        void OnSpawn();
        
        void OnRecycle();
    }

    public static class ObjectPools
    {
        private static Dictionary<Type, IObjectPool> s_Pools;

        static ObjectPools()
        {
            var baseType = typeof(IObjectPool);
            s_Pools = new Dictionary<Type, IObjectPool>();
            foreach (var type in Util_TypeCache.AllTypes)
            {
                if (!baseType.IsAssignableFrom(type))
                {
                    continue;
                }

                var attribute = type.GetCustomAttribute<CustomPoolAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                var pool = Activator.CreateInstance(type);
                s_Pools.Add(attribute.unitType, pool as IObjectPool);
            }
        }

        private static IObjectPool GetOrCreatePool(Type unitType)
        {
            if (!s_Pools.TryGetValue(unitType, out var pool))
            {
                var poolType = typeof(ObjectPool<>).MakeGenericType(unitType);
                s_Pools[unitType] = pool = (IObjectPool)Activator.CreateInstance(poolType);
            }

            return pool;
        }

        private static IObjectPool<T> GetOrCreatePool<T>() where T : class, new()
        {
            var unitType = typeof(T);
            if (!s_Pools.TryGetValue(unitType, out var pool))
            {
                s_Pools[unitType] = pool = new ObjectPool<T>();
            }

            return (IObjectPool<T>)pool;
        }

        public static IObjectPool GetPool(Type unitType)
        {
            return s_Pools.GetValueOrDefault(unitType);
        }

        public static void RegisterPool(Type unitType, IObjectPool pool)
        {
            s_Pools.Add(unitType, pool);
        }

        public static void ReleasePool(Type unitType)
        {
            var pool = GetPool(unitType);
            if (pool == null)
                return;

            pool.Dispose();
            s_Pools.Remove(unitType);
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
            if (!unitType.IsClass)
            {
                throw new TypeAccessException($"{unitType}不是引用类型");
            }

            var unit = GetOrCreatePool(unitType).Spawn();
            if (unit is IPoolableObject poolableObject)
            {
                poolableObject.OnSpawn();
            }
            return unit;
        }

        public static void Recycle(Type unitType, object unit)
        {
            if (!unitType.IsClass)
            {
                throw new TypeAccessException($"{unitType}不是引用类型");
            }
            
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