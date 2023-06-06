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
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */

#endregion

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CZToolKit.Common.UnsafeEx
{
    public unsafe static class UnsafeUtil
    {
        // public static int SizeOf<T>()
        // {
        //     return Unsafe.SizeOf<T>();
        // }
        
        public static IntPtr Malloc(int size)
        {
            return Marshal.AllocHGlobal(size);
        }

        public static void Free(IntPtr ptr)
        {
            Marshal.FreeHGlobal(ptr);
        }

        // public static void Wirte<T>(void* destination, T value)
        // {
        //     Unsafe.Write(destination, value);
        // }

        // public static void CopyPtrToStructure<T>(void* ptr, out T destination) where T : struct
        // {
        //     destination = Unsafe.Read<T>(ptr);
        // }
        //
        // public static T Read<T>(void* ptr)
        // {
        //     return Unsafe.Read<T>(ptr);
        // }
        //
        // public static ref T AsRef<T>(void* ptr)
        // {
        //     return ref Unsafe.AsRef<T>(ptr);
        // }
        //
        // public static ref T AsRef<T>(IntPtr ptr)
        // {
        //     return ref Unsafe.AsRef<T>((void*)ptr);
        // }
        //
        // public static ref T AsRef<T>(in T source)
        // {
        //     return ref Unsafe.AsRef(source);
        // }
        //
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
        //     Unsafe.Write(((byte*)destination + (Unsafe.SizeOf<T>() * index)), value);
        // }
        //
        // public static void CopyBlock(void* destination, void* source, uint byteCount)
        // {
        //     Unsafe.CopyBlock(destination, source, byteCount);
        // }
    }
}