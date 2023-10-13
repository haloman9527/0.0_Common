﻿using System;

namespace CZToolKit.ObjectPool
{
    public interface IObjectPool<T> : IDisposable where T : class
    {
        int UnusedCount { get; }

        T Spawn();
        void Recycle(T unit);
    }
}