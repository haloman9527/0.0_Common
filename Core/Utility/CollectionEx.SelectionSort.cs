using System;
using System.Collections.Generic;

namespace Atom
{
    public static partial class CollectionEx
    {
        /// <summary>
        /// 选择排序
        /// </summary>
        /// <param name="original"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns> 是否有变化 </returns>
        public static bool SelectionSort<T>(this IList<T> original, IComparer<T> comparer)
        {
            if (original.Count <= 1)
                return false;
            return SelectionSort(original, 0, original.Count - 1, comparer);
        }

        /// <summary>
        /// 选择排序
        /// </summary>
        /// <param name="original"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns> 是否有变化 </returns>
        public static bool SelectionSort<T>(this IList<T> original, int startIndex, int endIndex, IComparer<T> comparer)
        {
            var changed = false;
            for (int i = endIndex - 1; i >= startIndex; i--)
            {
                var minIndex = i;
                for (int j = i + 1; j <= endIndex; j++)  
                {  
                    if (comparer.Compare(original[minIndex], original[j]) > 0)  
                    {  
                        minIndex = j;  
                    }  
                }  
                
                if (minIndex != i)  
                {  
                    (original[i], original[minIndex]) = (original[minIndex], original[i]);  
                    changed = true;  
                }
            }

            return changed;
        }

        /// <summary>
        /// 选择排序
        /// </summary>
        /// <param name="original"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns> 是否有变化 </returns>
        public static bool SelectionSort<T>(this IList<T> original, Func<T, T, int> comparer)
        {
            if (original.Count <= 1)
                return false;
            return SelectionSort(original, 0, original.Count - 1, comparer);
        }

        /// <summary>
        /// 选择排序
        /// </summary>
        /// <param name="original"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns> 是否有变化 </returns>
        public static bool SelectionSort<T>(this IList<T> original, int startIndex, int endIndex, Func<T, T, int> comparer)
        {
            var changed = false;
            for (int i = endIndex - 1; i >= startIndex; i--)
            {
                var minIndex = i;
                for (int j = i + 1; j <= endIndex; j++)  
                {  
                    if (comparer(original[minIndex], original[j]) > 0)  
                    {  
                        minIndex = j;  
                    }  
                }  
                
                if (minIndex != i)  
                {  
                    (original[i], original[minIndex]) = (original[minIndex], original[i]);  
                    changed = true;  
                }
            }

            return changed;
        }

        public static unsafe bool SelectionSort<T>(T* original, int startIndex, int endIndex, IComparer<T> comparer) where T : unmanaged
        {
            var changed = false;
            for (int i = endIndex - 1; i >= startIndex; i--)
            {
                var minIndex = i;
                for (int j = i + 1; j <= endIndex; j++)  
                {  
                    if (comparer.Compare(original[minIndex], original[j]) > 0)  
                    {  
                        minIndex = j;  
                    }  
                }  
                
                if (minIndex != i)  
                {  
                    (original[i], original[minIndex]) = (original[minIndex], original[i]);  
                    changed = true;  
                }
            }

            return changed;
        }

        public static unsafe bool SelectionSort<T>(T* original, int startIndex, int endIndex, Func<T, T, int> comparer) where T : unmanaged
        {
            var changed = false;
            for (int i = endIndex - 1; i >= startIndex; i--)
            {
                var minIndex = i;
                for (int j = i + 1; j <= endIndex; j++)  
                {  
                    if (comparer(original[minIndex], original[j]) > 0)  
                    {  
                        minIndex = j;  
                    }  
                }  
                
                if (minIndex != i)  
                {  
                    (original[i], original[minIndex]) = (original[minIndex], original[i]);  
                    changed = true;  
                }
            }

            return changed;
        }
    }
}