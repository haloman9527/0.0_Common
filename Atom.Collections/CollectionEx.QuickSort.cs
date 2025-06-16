using System;
using System.Collections.Generic;

namespace Atom
{
    public static partial class CollectionEx
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
            
            var pivot = original[startIndex + (endIndex - startIndex) / 2];
            var left = startIndex;
            var right = endIndex;
            bool changed = false;
            while (left <= right)
            {
                while (comparer.Compare(original[left], pivot) < 0)
                {
                    left++;
                }

                while (comparer.Compare(original[right], pivot) > 0)
                {
                    right--;
                }

                if (left <= right)
                {
                    (original[left], original[right]) = (original[right], original[left]);
                    left++;
                    right--;
                    changed = true;
                }
            }

            if (startIndex < right)
            {
                changed |= QuickSort(original, startIndex, right, comparer);
            }

            if (left < endIndex)
            {
                changed |= QuickSort(original, left, endIndex, comparer);
            }

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
            
            var pivot = original[startIndex + (endIndex - startIndex) / 2];
            var left = startIndex;
            var right = endIndex;
            var changed = false;
            while (left <= right)
            {
                while (comparer(original[left], pivot) < 0)
                {
                    left++;
                }

                while (comparer(original[right], pivot) > 0)
                {
                    right--;
                }

                if (left <= right)
                {
                    (original[left], original[right]) = (original[right], original[left]);
                    left++;
                    right--;
                    changed = true;
                }
            }

            if (startIndex < right)
            {
                changed |= QuickSort(original, startIndex, right, comparer);
            }

            if (left < endIndex)
            {
                changed |= QuickSort(original, left, endIndex, comparer);
            }

            return changed;
        }

        public static unsafe bool QuickSort<T>(T* original, int startIndex, int endIndex, IComparer<T> comparer) where T : unmanaged
        {
            if (startIndex >= endIndex)
                return false;
            
            var pivot = original[startIndex + (endIndex - startIndex) / 2];
            int left = startIndex;
            int right = endIndex;
            var changed = false;
            while (left <= right)
            {
                while (comparer.Compare(original[left], pivot) < 0)
                {
                    left++;
                }

                while (comparer.Compare(original[right], pivot) > 0)
                {
                    right--;
                }

                if (left <= right)
                {
                    (original[left], original[right]) = (original[right], original[left]);
                    left++;
                    right--;
                    changed = true;
                }
            }

            if (startIndex < right)
            {
                changed |= QuickSort(original, startIndex, right, comparer);
            }

            if (left < endIndex)
            {
                changed |= QuickSort(original, left, endIndex, comparer);
            }

            return changed;
        }

        public static unsafe bool QuickSort<T>(T* original, int startIndex, int endIndex, Func<T, T, int> comparer) where T : unmanaged
        {
            if (startIndex >= endIndex)
                return false;
            T pivot = original[startIndex + (endIndex - startIndex) / 2];
            int left = startIndex;
            int right = endIndex;
            bool changed = false;
            while (left <= right)
            {
                while (comparer(original[left], pivot) < 0)
                {
                    left++;
                }

                while (comparer(original[right], pivot) > 0)
                {
                    right--;
                }

                if (left <= right)
                {
                    (original[left], original[right]) = (original[right], original[left]);
                    left++;
                    right--;
                    changed = true;
                }
            }

            if (startIndex < right)
            {
                changed |= QuickSort(original, startIndex, right, comparer);
            }

            if (left < endIndex)
            {
                changed |= QuickSort(original, left, endIndex, comparer);
            }

            return changed;
        }
    }
}