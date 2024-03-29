﻿using System.Collections.Generic;

namespace CZToolKit
{
    public abstract class BaseObjectPool<T> : IObjectPool<T> where T : class
    {
        protected Queue<T> unusedObjects;

        public int UnusedCount
        {
            get { return unusedObjects.Count; }
        }

        public BaseObjectPool()
        {
            this.unusedObjects = new Queue<T>();
        }

        object IObjectPool.Spawn()
        {
            return Spawn();
        }

        /// <summary> 生成 </summary>
        public T Spawn()
        {
            T unit = null;
            if (unusedObjects.Count > 0)
                unit = unusedObjects.Dequeue();
            else
                unit = Create();
            OnAcquire(unit);
            return unit;
        }

        void IObjectPool.Recycle(object unit)
        {
            Recycle(unit as T);
        }

        /// <summary> 回收 </summary>
        public void Recycle(T unit)
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

        protected virtual void Destroy(T unit)
        {
        }

        protected virtual void OnAcquire(T unit)
        {
        }

        protected virtual void OnRelease(T unit)
        {
        }
    }
}