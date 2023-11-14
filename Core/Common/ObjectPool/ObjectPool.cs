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
 *  Blog: https://www.mindgear.net/
 *
 */
#endregion
using System;
using System.Collections.Generic;

namespace CZToolKit.ObjectPool
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

        object IObjectPool.Acquire()
        {
            return Acquire();
        }
        
        /// <summary> 生成 </summary>
        public T Acquire()
        {
            T unit = null;
            if (unusedObjects.Count > 0)
                unit = unusedObjects.Dequeue();
            else
                unit = Create();
            OnAcquire(unit);
            return unit;
        }

        void IObjectPool.Release(object unit)
        {
            Release(unit as T);
        }

        /// <summary> 回收 </summary>
        public void Release(T unit)
        {
            unusedObjects.Enqueue(unit);
            OnRelease(unit);
        }

        public void Dispose()
        {
            while (unusedObjects.Count > 0)
            {
                Destroy(unusedObjects.Dequeue());
            }
        }

        protected abstract T Create();
        
        protected virtual void Destroy(T unit) { }
        
        protected virtual void OnAcquire(T unit) { }
        
        protected virtual void OnRelease(T unit) { }
    }
}