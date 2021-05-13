/*  注 释
 *
 *  Title: 
 *      "对象池管理器"
 *      
 *  Description:
 *      1.管理对象池
 *      2.清空所有对象池
 *      3.销毁所有对象池
 *  
 *  Date:       2020/11/19 10:00
 *  Version:    v1.0
 *  Writer:     半只龙虾人
 *
 */

using System.Collections.Generic;
using CZToolKit.Core.Singletons;

namespace CZToolKit.Core.ObjectPool
{
    public class PoolManager : CZNormalSingleton<PoolManager>
    {
        private Dictionary<string, IPoolBase> _pools = new Dictionary<string, IPoolBase>();

        public bool TryGetPool<T>(string _poolName, out T _pool) where T : class, IPoolBase, new()
        {
            IPoolBase pool;
            _pool = null;
            if (_pools.TryGetValue(_poolName, out pool))
            {
                _pool = pool as T;
                if (_pool != null)
                    return true;
            }
            return false;
        }

        public bool Contains(string _poolName)
        {
            return _pools[_poolName] != null;
        }

        public void SetPool(string _poolName, IPoolBase _pool)
        {
            _pools[_poolName] = _pool;
        }

        public void RemovePool(string poolName)
        {
            _pools.Remove(poolName);
        }
    }
}