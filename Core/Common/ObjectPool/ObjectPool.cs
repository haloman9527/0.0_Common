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

namespace CZToolKit
{
    public class ObjectPool<T> : BaseObjectPool<T> where T : class, new()
    {
        protected override T Create()
        {
            return new T();
        }
    }

    public class ObjectPool : AutoSingleton<ObjectPool>, ISingletonAwake
    {
        private Dictionary<Type, IObjectPool> pools;

        public void Awake()
        {
            pools = new Dictionary<Type, IObjectPool>();
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

        public IObjectPool GetPool(Type unitType)
        {
            if (!pools.TryGetValue(unitType, out var pool))
            {
                return null;
            }

            return pool;
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
            return (T)GetOrCreatePool(typeof(T)).Spawn();
        }

        public object Spawn(Type unitType)
        {
            if (unitType.IsValueType)
            {
                throw new Exception("Can't spawn value type");
            }
            
            var pool = GetPool(unitType);
            if (pool == null)
            {
                var constructor = unitType.GetConstructor(Type.EmptyTypes);
                if (constructor != null)
                    pool = GetOrCreatePool(unitType);
            }

            if (pool == null)
            {
                throw new Exception($"Can't spawn {unitType.Name}, please register the pool first");
            }
            else
            {
                return GetOrCreatePool(unitType).Spawn();
            }
        }

        public void Recycle(object reference)
        {
            var unitType = reference.GetType();
            GetOrCreatePool(unitType).Recycle(reference);
        }
    }
}