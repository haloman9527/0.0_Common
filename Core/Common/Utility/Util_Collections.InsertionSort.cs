using System;
using System.Collections.Generic;

namespace CZToolKit
{
    public static partial class Util_Collections
    {
        /// <summary>
        /// 插入排序
        /// </summary>
        /// <param name="original"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns> 是否有变化 </returns>
        public static bool InsertionSort<T>(this IList<T> original, IComparer<T> comparer)
        {
            if (original.Count <= 1)
                return false;
            return InsertionSort(original, 0, original.Count - 1, comparer);
        }

        /// <summary>
        /// 插入排序
        /// </summary>
        /// <param name="original"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns> 是否有变化 </returns>
        public static bool InsertionSort<T>(this IList<T> original, int startIndex, int endIndex, IComparer<T> comparer)
        {
            var changed = false;
            for (int i = endIndex - 1; i >= startIndex; i--)
            {
                var index = i;
                var temp = original[index];

                for (int j = i + 1; j <= endIndex; j++)
                {
                    var t = original[j];
                    if (comparer.Compare(temp, t) > 0)
                    {
                        original[index] = t;
                        original[j] = temp;
                        changed |= true;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return changed;
        }

        /// <summary>
        /// 插入排序
        /// </summary>
        /// <param name="original"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns> 是否有变化 </returns>
        public static bool InsertionSort<T>(this IList<T> original, Func<T, T, int> comparer)
        {
            if (original.Count <= 1)
                return false;
            return InsertionSort(original, 0, original.Count - 1, comparer);
        }

        /// <summary>
        /// 插入排序
        /// </summary>
        /// <param name="original"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns> 是否有变化 </returns>
        public static bool InsertionSort<T>(this IList<T> original, int startIndex, int endIndex, Func<T, T, int> comparer)
        {
            var changed = false;
            for (int i = endIndex - 1; i >= startIndex; i--)
            {
                var index = i;
                var temp = original[index];

                for (int j = i + 1; j <= endIndex; j++)
                {
                    var t = original[j];
                    if (comparer(temp, t) > 0)
                    {
                        original[index] = t;
                        original[j] = temp;
                        changed |= true;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return changed;
        }

        public static unsafe bool InsertionSort<T>(T* original, int startIndex, int endIndex, IComparer<T> comparer) where T : unmanaged
        {
            var changed = false;
            for (int i = endIndex - 1; i >= startIndex; i--)
            {
                var index = i;
                var temp = original[index];

                for (int j = i + 1; j <= endIndex; j++)
                {
                    var t = original[j];
                    if (comparer.Compare(temp, t) > 0)
                    {
                        original[index] = t;
                        original[j] = temp;
                        changed |= true;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return changed;
        }

        public static unsafe bool InsertionSort<T>(T* original, int startIndex, int endIndex, Func<T, T, int> comparer) where T : unmanaged
        {
            var changed = false;
            for (int i = endIndex - 1; i >= startIndex; i--)
            {
                var index = i;
                var temp = original[index];

                for (int j = i + 1; j <= endIndex; j++)
                {
                    var t = original[j];
                    if (comparer(temp, t) > 0)
                    {
                        original[index] = t;
                        original[j] = temp;
                        changed |= true;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return changed;
        }
    }
}