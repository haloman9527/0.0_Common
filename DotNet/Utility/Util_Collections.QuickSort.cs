using System;
using System.Collections.Generic;

namespace CZToolKit
{
    public static partial class Util_Collections
    {
        /// <summary>
        /// 快速排序
        /// </summary>
        /// <param name="original"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns> 是否有变化 </returns>
        public static bool QuickSort<T>(this IList<T> original, IComparer<T> comparer)
        {
            if (original.Count <= 1)
                return false;
            return QuickSort(original, 0, original.Count - 1, comparer);
        }

        /// <summary>
        /// 快速排序
        /// </summary>
        /// <param name="original"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns> 是否有变化 </returns>
        public static bool QuickSort<T>(this IList<T> original, int startIndex, int endIndex, IComparer<T> comparer)
        {
            if (startIndex >= endIndex)
                return false;
            T middle = original[startIndex];
            int less = startIndex;
            int greater = endIndex;
            bool changed = false;
            while (true)
            {
                while (less < greater && comparer.Compare(original[less], middle) < 0)
                {
                    less++;
                }

                while (greater > less && comparer.Compare(original[greater], middle) > 0)
                {
                    greater--;
                }

                if (less >= greater) break;

                var lr = comparer.Compare(original[less], original[greater]);
                if (lr == 0)
                {
                    greater--;
                    continue;
                }

                T temp = original[less];
                original[less] = original[greater];
                original[greater] = temp;
                changed = true;
            }

            changed |= QuickSort(original, startIndex, less, comparer);
            changed |= QuickSort(original, less + 1, endIndex, comparer);
            return changed;
        }

        /// <summary>
        /// 快速排序
        /// </summary>
        /// <param name="original"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns> 是否有变化 </returns>
        public static bool QuickSort<T>(this IList<T> original, Func<T, T, int> comparer)
        {
            if (original.Count <= 1)
                return false;
            return QuickSort(original, 0, original.Count - 1, comparer);
        }

        /// <summary>
        /// 快速排序
        /// </summary>
        /// <param name="original"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns> 是否有变化 </returns>
        public static bool QuickSort<T>(this IList<T> original, int startIndex, int endIndex, Func<T, T, int> comparer)
        {
            if (startIndex >= endIndex)
                return false;
            T middle = original[startIndex];
            int less = startIndex;
            int greater = endIndex;
            bool changed = false;
            while (true)
            {
                while (less < greater && comparer(original[less], middle) < 0)
                {
                    less++;
                }

                while (greater > less && comparer(original[greater], middle) > 0)
                {
                    greater--;
                }

                if (less >= greater) break;

                var lr = comparer(original[less], original[greater]);
                if (lr == 0)
                {
                    greater--;
                    continue;
                }

                T temp = original[less];
                original[less] = original[greater];
                original[greater] = temp;
                changed = true;
            }

            changed |= QuickSort(original, startIndex, less, comparer);
            changed |= QuickSort(original, less + 1, endIndex, comparer);
            return changed;
        }

        public static unsafe bool QuickSort<T>(T* original, int startIndex, int endIndex, IComparer<T> comparer) where T : unmanaged
        {
            if (startIndex >= endIndex)
                return false;
            T middle = original[startIndex];
            int less = startIndex;
            int greater = endIndex;
            bool changed = false;
            while (true)
            {
                while (less < greater && comparer.Compare(original[less], middle) < 0)
                {
                    less++;
                }

                while (greater > less && comparer.Compare(original[greater], middle) > 0)
                {
                    greater--;
                }

                if (less >= greater) break;

                var lr = comparer.Compare(original[less], original[greater]);
                if (lr == 0)
                {
                    greater--;
                    continue;
                }

                T temp = original[less];
                original[less] = original[greater];
                original[greater] = temp;
                changed = true;
            }

            changed |= QuickSort(original, startIndex, less, comparer);
            changed |= QuickSort(original, less + 1, endIndex, comparer);
            return changed;
        }

        public static unsafe bool QuickSort<T>(T* original, int startIndex, int endIndex, Func<T, T, int> comparer) where T : unmanaged
        {
            if (startIndex >= endIndex)
                return false;
            T middle = original[startIndex];
            int less = startIndex;
            int greater = endIndex;
            bool changed = false;
            while (true)
            {
                while (less < greater && comparer(original[less], middle) < 0)
                {
                    less++;
                }

                while (greater > less && comparer(original[greater], middle) > 0)
                {
                    greater--;
                }

                if (less >= greater) break;

                var lr = comparer(original[less], original[greater]);
                if (lr == 0)
                {
                    greater--;
                    continue;
                }

                T temp = original[less];
                original[less] = original[greater];
                original[greater] = temp;
                changed = true;
            }

            changed |= QuickSort(original, startIndex, less, comparer);
            changed |= QuickSort(original, less + 1, endIndex, comparer);
            return changed;
        }
    }
}