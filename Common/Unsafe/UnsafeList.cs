using System;
using Unity.Collections;

namespace CZToolKit.Common.UnsafeEx
{
    public unsafe struct UnsafeList<T> : IDisposable where T : unmanaged
    {
        public T** ptr;
        public int capacity;
        public int count;
        public Allocator allocator;

        public UnsafeList(Allocator allocator)
        {
            ptr = null;
            capacity = 0;
            count = 0;
            this.allocator = allocator;
        }

        // public UnsafeList(T* ptr, int count)
        // {
        //     this.ptr = (T**)ptr;
        //     this.capacity = count;
        //     this.count = count;
        //     this.allocator = Allocator.None;
        // }

        public void Grow(int nextCapacity)
        {
            var nextBufferSize = (ulong)(sizeof(T*) * nextCapacity);
            ptr = (T**)UnsafeUtil.Malloc((int)nextBufferSize);
            
        }
        
        public void Dispose()
        {
        }
    }
}