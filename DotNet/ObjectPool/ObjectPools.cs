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

    public class ObjectPools : Singleton<ObjectPools>, ISingletonAwake
    {
        private static Dictionary<Type, IObjectPool> s_CustomPools;

        static ObjectPools()
        {
            var baseType = typeof(IObjectPool);
            s_CustomPools = new Dictionary<Type, IObjectPool>();
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
                s_CustomPools.Add(attribute.unitType, pool as IObjectPool);
            }
        }

        private Dictionary<Type, IObjectPool> pools;

        public void Awake()
        {
            pools = new Dictionary<Type, IObjectPool>();
            foreach (var pair in s_CustomPools)
            {
                pools.Add(pair.Key, pair.Value);
            }
        }

        private IObjectPool GetOrCreatePool(Type unitType)
        {
            if (!pools.TryGetValue(unitType, out var pool))
            {
                var poolType = typeof(ObjectPool<>).MakeGenericType(unitType);
                pools[unitType] = pool = (IObjectPool)Activator.CreateInstance(poolType);
            }

            return pool;
        }

        private IObjectPool<T> GetOrCreatePool<T>() where T : class, new()
        {
            var unitType = typeof(T);
            if (!pools.TryGetValue(unitType, out var pool))
            {
                pools[unitType] = pool = new ObjectPool<T>();
            }

            return (IObjectPool<T>)pool;
        }

        public IObjectPool GetPool(Type unitType)
        {
            return pools.GetValueOrDefault(unitType);
        }

        public void RegisterPool(Type unitType, IObjectPool pool)
        {
            pools.Add(unitType, pool);
        }

        public void ReleasePool(Type unitType)
        {
            var pool = GetPool(unitType);
            if (pool == null)
                return;

            pool.Dispose();
            pools.Remove(unitType);
        }

        public T Spawn<T>() where T : class, new()
        {
            var unit = GetOrCreatePool<T>().Spawn();
            if (unit is IPoolableObject poolableObject)
            {
                poolableObject.OnSpawn();
            }
            return unit;
        }

        public object Spawn(Type unitType)
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

        public void Recycle(Type unitType, object unit)
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

        public void Recycle(object unit)
        {
            Recycle(unit.GetType(), unit);
        }
    }
}