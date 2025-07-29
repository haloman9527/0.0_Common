using System;

namespace Atom
{
    public interface IObjectPool
    {
        /// <summary>
        /// 对象池对象类型
        /// </summary>
        Type ObjectType { get; }

        /// <summary>
        /// 对象池中缓存的对象数量
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <returns></returns>
        object Spawn();

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="obj">要回收的对象</param>
        void Recycle(object obj);

        /// <summary>
        /// 尝试释放<see cref="toReleaseCount"/>数量的对象
        /// </summary>
        void Release(int toReleaseCount);

        /// <summary>
        /// 释放所有未使用的对象
        /// </summary>
        void ReleaseAll();
    }

    public interface IObjectPool<T> : IObjectPool where T : class
    {
        /// <summary>
        /// 对象池对象类型
        /// </summary>
        Type ObjectType { get; }

        /// <summary>
        /// 对象池中缓存的对象数量
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <returns></returns>
        T Spawn();

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="obj">要回收的对象</param>
        void Recycle(T obj);

        /// <summary>
        /// 尝试释放<see cref="toReleaseCount"/>数量的对象
        /// </summary>
        void Release(int toReleaseCount);

        /// <summary>
        /// 释放所有未使用的对象
        /// </summary>
        void ReleaseAll();
    }
}