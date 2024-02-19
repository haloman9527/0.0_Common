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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CZToolKit.UnsafeEx
{
    public unsafe static class UnsafeUtil
    {
        public static int SizeOf<T>()
        {
            return Marshal.SizeOf<T>();
        }
        
        public static IntPtr Malloc(int size)
        {
            return Marshal.AllocHGlobal(size);
        }

        public static void Free(IntPtr ptr)
        {
            Marshal.FreeHGlobal(ptr);
        }
        //
        // public static void Wirte<T>(void* dest, T value)
        // {
        //     // Unsafe.Write(destination, value);
        // }
        //
        // public static void CopyPtrToStructure<T>(void* ptr, out T dest) where T : struct
        // {
        //     // destination = Unsafe.Read<T>(ptr);
        //     dest = default;
        // }
        
        public static T Read<T>(void* ptr) where T : unmanaged
        {
            return *(T*)ptr;
        }
        
        public static ref T AsRef<T>(void* ptr) where T : unmanaged
        {
            return ref *(T*)ptr;
        }
        
        public static ref T AsRef<T>(IntPtr ptr) where T : unmanaged
        {
            return ref *(T*)ptr;
        }
        
        // public static ref TTo As<TFrom, TTo>(ref TFrom from)
        // {
        //     return ref Unsafe.As<TFrom, TTo>(ref from);
        // }
        //
        // public static T ReadArrayElement<T>(void* destination, int index)
        // {
        //     return Unsafe.Read<T>((byte*)destination + (Unsafe.SizeOf<T>() * index));
        // }
        //
        // public static void WirteArrayElement<T>(void* destination, int index, T value)
        // {
        //     Unsafe.Write(((byte*)destination + (SizeOf<T>() * index)), value);
        // }
        
        public static void CopyBlock(void* destination, void* source, uint byteCount)
        {
            Buffer.MemoryCopy(source, destination, byteCount, byteCount);
        }
    }
}