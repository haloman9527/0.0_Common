#region 注 释

/***
 *
 *  Title:
 *
 *  Description:
 *
 *  Date:
 *  Version:
 *  Writer: 半只龙虾人
 *  Github: https://github.com/haloman9527
 *  Blog: https://www.haloman.net/
 *
 */

#endregion

using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace CZToolKit.UnsafeEx
{
    public unsafe static class UnsafeUtil
    {
        public static int AlignOf<T>() where T : struct
        {
            return UnsafeUtility.AlignOf<T>();
        }

        public static int SizeOf<T>()
        {
            return Marshal.SizeOf<T>();
        }

        public static int SizeOf(Type type)
        {
            return Marshal.SizeOf(type);
        }

        public static IntPtr Malloc(int size)
        {
            return Marshal.AllocHGlobal(size);
        }

        public static void* Malloc(long size, int alignment, Allocator allocator)
        {
            return UnsafeUtility.Malloc(size, alignment, allocator);
        }

        public static void Free(IntPtr ptr)
        {
            Marshal.FreeHGlobal(ptr);
        }

        public static void Free(void* ptr, Allocator allocator)
        {
            UnsafeUtility.Free(ptr, allocator);
        }

        public static void Wirte<T>(void* dest, T value) where T : unmanaged
        {
            ((T*)dest)[0] = value;
        }

        public static void CopyPtrToStructure<T>(void* ptr, out T dest) where T : unmanaged
        {
            dest = Marshal.PtrToStructure<T>((IntPtr)ptr);
        }

        public static T Read<T>(void* ptr) where T : unmanaged
        {
            return *(T*)ptr;
        }

        public static object Read(IntPtr ptr)
        {
            return GCHandle.FromIntPtr(ptr).Target;
        }

        public static ref T AsRef<T>(void* ptr) where T : unmanaged
        {
            return ref *(T*)ptr;
        }

        public static ref T AsRef<T>(IntPtr ptr) where T : unmanaged
        {
            return ref *(T*)ptr;
        }

        public static ref TTo As<TFrom, TTo>(ref TFrom from) where TFrom : unmanaged where TTo : unmanaged
        {
            fixed (TFrom* ptr = &from)
            {
                return ref *(TTo*)ptr;
            }
        }

        public static T ReadArrayElement<T>(void* destination, int index) where T : unmanaged
        {
            return Read<T>((byte*)destination + (SizeOf<T>() * index));
        }

        public static void WirteArrayElement<T>(void* destination, int index, T value) where T : unmanaged
        {
            Wirte(((byte*)destination + (SizeOf<T>() * index)), value);
        }

        public static void CopyBlock(void* destination, void* source, uint byteCount)
        {
            Buffer.MemoryCopy(source, destination, byteCount, byteCount);
        }

        public static void CopyStructureToPtr<T>(ref T input, void* ptr) where T : struct
        {
            UnsafeUtility.CopyStructureToPtr(ref input, ptr);
        }
    }
}