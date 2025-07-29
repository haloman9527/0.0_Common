using System;
using System.Collections.Generic;

namespace Atom
{
    public static partial class ObjectPoolManager
    {
        private sealed class ObjectPool<T> : IObjectPool<T> where T : class, new()
        {
            private Queue<T> m_CachedObjects;

            public Type ObjectType
            {
                get { return typeof(T); }
            }

            public int Count
            {
                get { return m_CachedObjects.Count; }
            }

            public ObjectPool()
            {
                m_CachedObjects = new Queue<T>(8);
            }

            object IObjectPool.Spawn()
            {
                return Spawn();
            }

            public T Spawn()
            {
                T obj = m_CachedObjects.Count > 0 ? m_CachedObjects.Dequeue() : new T();
                if (obj is IObjectPoolable iObj)
                {
                    iObj.OnSpawn();
                }
                
                return obj;
            }

            void IObjectPool.Recycle(object obj)
            {
                if (obj == null)
                {
                    throw new ArgumentNullException(nameof(obj));
                }
                
                Recycle((T)obj);
            }

            public void Recycle(T obj)
            {
                if (obj == null)
                {
                    throw new ArgumentNullException(nameof(obj));
                }
                
                m_CachedObjects.Enqueue(obj);
                if (obj is IObjectPoolable iObj)
                {
                    iObj.OnRecycle();
                }
            }

            public void Release(int toReleaseCount)
            {
                while (toReleaseCount-- > 0 && m_CachedObjects.Count > 0)
                {
                    m_CachedObjects.Dequeue();
                }
            }

            public void ReleaseAll()
            {
                while (m_CachedObjects.Count > 0)
                {
                    m_CachedObjects.Dequeue();
                }
            }
        }
    }
}