using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Atom.LowLevel
{
    /// <summary>
    /// 提供常用低级内存操作API，适用于高性能和非托管场景。
    /// </summary>
    public unsafe static class Unsafe
    {
#if !UNITY_WEBGL
        /// <summary>
        /// 分配非托管内存。
        /// </summary>
        public static IntPtr AllocHGlobal(int size)
        {
            return Marshal.AllocHGlobal(size);
        }

        /// <summary>
        /// 释放非托管内存。
        /// </summary>
        public static void FreeHGlobal(IntPtr ptr)
        {
            Marshal.FreeHGlobal(ptr);
        }

        /// <summary>
        /// 使用Unity UnsafeUtility分配内存。
        /// </summary>
        public static void* Malloc(long size, int alignment, Allocator allocator)
        {
            return UnsafeUtility.Malloc(size, alignment, allocator);
        }

        /// <summary>
        /// 使用Unity UnsafeUtility释放内存。
        /// </summary>
        public static void Free(void* ptr, Allocator allocator)
        {
            UnsafeUtility.Free(ptr, allocator);
        }

        /// <summary>
        /// 获取变量的指针（需在unsafe上下文中）。
        /// </summary>
        public static T* AddressOf<T>(ref T value) where T : unmanaged
        {
            fixed (T* ptr = &value) return ptr;
        }

        /// <summary>
        /// 读取指针处的值。
        /// </summary>
        public static T Read<T>(void* ptr) where T : unmanaged
        {
            return *(T*)ptr;
        }

        /// <summary>
        /// 向指针处写入值。
        /// </summary>
        public static void Write<T>(void* ptr, T value) where T : unmanaged
        {
            ((T*)ptr)[0] = value;
        }

        /// <summary>
        /// 读取IntPtr处的值。
        /// </summary>
        public static T Read<T>(IntPtr ptr) where T : unmanaged
        {
            return *(T*)ptr;
        }

        /// <summary>
        /// 读取结构体（从IntPtr）。
        /// </summary>
        public static T PtrToStructure<T>(IntPtr ptr) where T : struct
        {
            return Marshal.PtrToStructure<T>(ptr);
        }

        /// <summary>
        /// 将结构体写入IntPtr。
        /// </summary>
        public static void StructureToPtr<T>(T value, IntPtr ptr, bool fDeleteOld) where T : struct
        {
            Marshal.StructureToPtr(value, ptr, fDeleteOld);
        }

        /// <summary>
        /// 读取数组元素。
        /// </summary>
        public static T ReadArrayElement<T>(void* ptr, int index) where T : unmanaged
        {
            return UnsafeUtility.ReadArrayElement<T>(ptr, index);
        }

        /// <summary>
        /// 写入数组元素。
        /// </summary>
        public static void WriteArrayElement<T>(void* ptr, int index, T value) where T : unmanaged
        {
            UnsafeUtility.WriteArrayElement<T>(ptr, index, value);
        }

        /// <summary>
        /// 拷贝内存块。
        /// </summary>
        public static void MemCpy(void* dest, void* src, long size)
        {
            UnsafeUtility.MemCpy(dest, src, size);
        }

        /// <summary>
        /// 清零内存块。
        /// </summary>
        public static void MemClear(void* ptr, long size)
        {
            UnsafeUtility.MemClear(ptr, size);
        }

        /// <summary>
        /// 获取类型的大小（字节）。
        /// </summary>
        public static int SizeOf<T>() where T : struct
        {
            return UnsafeUtility.SizeOf<T>();
        }

        /// <summary>
        /// 获取类型的对齐（字节）。
        /// </summary>
        public static int AlignOf<T>() where T : struct
        {
            return UnsafeUtility.AlignOf<T>();
        }

        /// <summary>
        /// 获取字段在结构体中的偏移。
        /// </summary>
        public static int GetFieldOffset(FieldInfo field)
        {
            return UnsafeUtility.GetFieldOffset(field);
        }

        /// <summary>
        /// 判断类型是否为非托管类型。
        /// </summary>
        public static bool IsUnmanaged<T>()
        {
            return UnsafeUtility.IsUnmanaged<T>();
        }

        /// <summary>
        /// 判断Type是否为非托管类型。
        /// </summary>
        public static bool IsUnmanaged(Type type)
        {
            return UnsafeUtility.IsUnmanaged(type);
        }

        /// <summary>
        /// 获取引用类型对象的堆地址。
        /// </summary>
        public static IntPtr GetObjectAddr(object obj)
        {
            TypedReference tr = __makeref(obj);
            return *(IntPtr*)*(IntPtr*)&tr;
        }
#endif
    }
} 