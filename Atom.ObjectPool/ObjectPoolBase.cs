using System;
using System.Collections.Generic;

namespace Atom
{
    public abstract class ObjectPoolBase<T> : IObjectPool<T> where T : class
    {
        protected Queue<T> m_CachedObjects;
        protected int m_Capacity;

        public ObjectPoolBase()
        {
            m_CachedObjects = new Queue<T>(16);
            m_Capacity = int.MaxValue;
        }

        public ObjectPoolBase(int initialCapacity)
        {
            m_CachedObjects = new Queue<T>(16);
            if (initialCapacity <= 0)
            {
                throw new ArgumentException("InitialCapacity must be greater than zero.");
            }
            m_Capacity = initialCapacity;
        }

        public int Count
        {
            get { return m_CachedObjects.Count; }
        }

        public int Capacity
        {
            get { return m_Capacity; }
            set
            {
                if (value < 0)
                    throw new Exception("Capacity is invalid.");
                if (m_Capacity == value)
                    return;
                m_Capacity = value;
                Release();
            }
        }

        public Type ObjectType
        {
            get { return TypeCache<T>.TYPE; }
        }

        object IObjectPool.Spawn()
        {
            return Spawn();
        }

        public T Spawn()
        {
            T obj = m_CachedObjects.Count > 0 ? m_CachedObjects.Dequeue() : Create();
            OnSpawn(obj);
            return obj;
        }

        void IObjectPool.Recycle(object obj)
        {
            Recycle((T)obj);
        }

        public void Recycle(T obj)
        {
            m_CachedObjects.Enqueue(obj);
            OnRecycle(obj);
        }

        public void Release()
        {
            Release(Count - m_Capacity);
        }

        public void Release(int toReleaseCount)
        {
            while (toReleaseCount-- > 0 && m_CachedObjects.Count > 0)
            {
                OnRelease(m_CachedObjects.Dequeue());
            }
        }

        public void ReleaseAll()
        {
            while (m_CachedObjects.Count > 0)
            {
                OnRelease(m_CachedObjects.Dequeue());
            }
        }

        protected abstract T Create();

        protected virtual void OnSpawn(T obj)
        {
        }

        protected virtual void OnRecycle(T obj)
        {
        }

        protected virtual void OnRelease(T obj)
        {
        }
    }
}