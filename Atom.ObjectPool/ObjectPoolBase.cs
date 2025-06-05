using System;
using System.Collections.Generic;

namespace Atom
{
    public abstract class ObjectPoolBase<T> : IObjectPool, IObjectPool<T> where T : class
    {
        protected Queue<T> unusedObjects;
        
        public Type UnitType => TypeCache<T>.TYPE;

        public int UnusedCount => unusedObjects.Count;

        public ObjectPoolBase()
        {
            this.unusedObjects = new Queue<T>(16);
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

        public void Release()
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