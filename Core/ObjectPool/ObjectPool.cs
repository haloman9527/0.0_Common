using System;
using System.Collections.Generic;

namespace Moyo
{
    public abstract class ObjectPool<T> : IObjectPool, IObjectPool<T> where T : class
    {
        protected Queue<T> unusedObjects;
        
        public Type UnitType => TypeCache<T>.TYPE;

        public int UnusedCount => unusedObjects.Count;
        protected virtual int InitNum => 8;

        public ObjectPool()
        {
            this.unusedObjects = new Queue<T>(InitNum);
        }

        /// <summary> 生成 </summary>
        public T Spawn()
        {
            T unit = null;
            if (unusedObjects.Count > 0)
                unit = unusedObjects.Dequeue();
            else
                unit = Create();
            OnSpawn(unit);
            return unit;
        }

        /// <summary> 回收 </summary>
        public void Recycle(T unit)
        {
            unusedObjects.Enqueue(unit);
            OnRecycle(unit);
        }

        object IObjectPool.Spawn()
        {
            return Spawn();
        }

        void IObjectPool.Recycle(object unit)
        {
            Recycle(unit as T);
        }

        public void Dispose()
        {
            while (unusedObjects.Count > 0)
            {
                OnDestroy(unusedObjects.Dequeue());
            }
        }

        protected abstract T Create();

        protected virtual void OnDestroy(T unit)
        {
        }

        protected virtual void OnSpawn(T unit)
        {
        }

        protected virtual void OnRecycle(T unit)
        {
        }
    }
}