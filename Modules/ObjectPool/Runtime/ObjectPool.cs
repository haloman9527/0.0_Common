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

namespace CZToolKit.Core.ObjectPool
{
    public interface IPoolBase { }

    [Serializable]
    public class ObjectPool<T> : IPoolBase, IDisposable where T : class
    {
        public const int DEFAULT_SIZE = 32;

        internal readonly int maxSize;
        internal readonly Queue<T> idleQueue;
        protected Func<T> createFunction;
        protected Action<T> onSpawn;
        protected Action<T> onRelease;
        protected Action<T> destroyAction;

        public int Count
        {
            get; private set;
        }
        public int ActiveCount
        {
            get { return Count - InactiveCount; }
        }
        public int InactiveCount
        {
            get { return idleQueue.Count; }
        }

        protected ObjectPool(int capacity = 16, int maxSize = 10000)
        {
            this.maxSize = maxSize;
            this.idleQueue = new Queue<T>(capacity);
        }

        public ObjectPool(Func<T> createFunction, Action<T> onSpawn, Action<T> onRelease, Action<T> destroyAction, int capacity = 16, int maxSize = 10000) : this(capacity, maxSize)
        {
            this.createFunction = createFunction;
            this.onSpawn = onSpawn;
            this.onRelease = onRelease;
            this.destroyAction = destroyAction;
        }

        /// <summary> 生成 </summary>
        public T Spawn()
        {
            T unit = null;
            if (idleQueue.Count == 0)
            {
                unit = createFunction();
                Count++;
            }
            else
                unit = idleQueue.Dequeue();
            onSpawn?.Invoke(unit);
            return unit;
        }

        /// <summary> 释放 </summary>
        public void Release(T unit)
        {
            if (InactiveCount <= maxSize)
            {
                idleQueue.Enqueue(unit);
                onRelease?.Invoke(unit);
            }
            else
            {
                destroyAction?.Invoke(unit);
                Count--;
            }
        }

        public void Dispose()
        {
            foreach (T unit in idleQueue)
            {
                destroyAction?.Invoke(unit);
            }
            idleQueue.Clear();
            Count = 0;
        }
    }
}