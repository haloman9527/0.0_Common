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
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */
#endregion
using System;
using System.Collections.Generic;

namespace CZToolKit.Common.ObjectPool
{
    [Serializable]
    public abstract class ObjectPool<T> : IObjectPool<T> where T : class
    {
        private Queue<T> unusedObjects;
        
        public int UnusedCount
        {
            get { return unusedObjects.Count; }
        }

        public ObjectPool()
        {
            this.unusedObjects = new Queue<T>();
        }

        /// <summary> 生成 </summary>
        public T Spawn()
        {
            T unit = null;
            if (unusedObjects.Count > 0)
                unit = unusedObjects.Dequeue();
            else
                unit = Generate();
            OnSpawn(unit);
            return unit;
        }

        /// <summary> 回收 </summary>
        public void Recycle(T unit)
        {
            unusedObjects.Enqueue(unit);
            OnRecycle(unit);
        }

        public void Release()
        {
            foreach (T unit in unusedObjects)
            {
                Release(unit);
            }
            unusedObjects.Clear();
        }

        protected abstract T Generate();
        
        protected virtual void Release(T unit) { }

        protected virtual void OnSpawn(T unit) { }
        
        protected virtual void OnRecycle(T unit) { }
    }
}