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
 *  Blog: https://www.mindgear.net/
 *
 */
#endregion
using System;
using System.Collections.Generic;
using CZToolKit.Singletons;

namespace CZToolKit
{
    public class ObjectPool<T> : BaseObjectPool<T> where T : class, new()
    {
        protected override T Create()
        {
            return new T();
        }
    }

    public class ObjectPool : AutoSingleton<ObjectPool> , ISingletonAwake
    {
        private Dictionary<Type, IObjectPool> pools;

        public void Awake()
        {
            pools = new Dictionary<Type, IObjectPool>();
        }

        private IObjectPool GetPool(Type unitType)
        {
            if (!pools.TryGetValue(unitType, out var pool))
            {
                var poolType = typeof(ObjectPool<>).MakeGenericType(unitType);
                pools[unitType] = pool = (IObjectPool)Activator.CreateInstance(poolType);
            }
            return pool;
        }
        
        public T Acquire<T>() where T : class, new()
        {
            return (T)GetPool(typeof(T)).Spawn();
        }
        
        public object Acquire(Type unitType)
        {
            return GetPool(unitType).Spawn();
        }

        public void Recycle(object reference)
        {
            var unitType = reference.GetType(); 
            GetPool(unitType).Recycle(reference);
        }
    }
}