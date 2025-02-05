using System;
using System.Collections.Generic;

namespace Moyo
{
    public static partial class CollectionEx
    {
        /// <summary>
        /// 二分匹配, 返回item在数组中的合适索引
        /// </summary>
        /// <param name="original"></param>
        /// <param name="item"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int BinaryMatch<T>(this IList<T> original, T item, Func<T, T, int> comparer)
        {
            return BinaryMatch(original, 0, original.Count - 1, item, comparer);
        }

        public static int BinaryMatch<T>(this IList<T> original, int startIndex, int endIndex, T item, Func<T, T, int> comparer)
        {
            if (original.Count == 0)
                return -1;

            while (startIndex <= endIndex)
            {
                var mid = startIndex + (endIndex - startIndex) / 2;
                var dir = comparer(item, original[mid]);
                if (dir == 0)
                    return mid;

                if (startIndex == endIndex)
                {
                    if (dir < 0)
                        return Math.Clamp(mid, 0, original.Count);
                    else
                        return Math.Clamp(mid + 1, 0, original.Count);
                }
                else
                {
                    if (dir < 0)
                        endIndex = mid - 1;
                    else
                        startIndex = mid + 1;
                }
            }

            return -1;
        }

        public static int BinaryMatch<T>(this IList<T> original, T item, IComparer<T> comparer)
        {
            return BinaryMatch(original, 0, original.Count - 1, item, comparer);
        }

        public static int BinaryMatch<T>(this IList<T> original, int startIndex, int endIndex, T item, IComparer<T> comparer)
        {
            if (original.Count == 0)
                return -1;

            while (startIndex <= endIndex)
            {
                var mid = startIndex + (endIndex - startIndex) / 2;
                var dir = comparer.Compare(item, original[mid]);
                if (dir == 0)
                    return mid;

                if (startIndex == endIndex)
                {
                    if (dir < 0)
                        return Math.Clamp(mid, 0, original.Count);
                    else
                        return Math.Clamp(mid + 1, 0, original.Count);
                }
                else
                {
                    if (dir < 0)
                        endIndex = mid - 1;
                    else
                        startIndex = mid + 1;
                }
            }

            return -1;
        }

        /// <summary>
        /// 二分查找，返回item在数组中的索引
        /// </summary>
        /// <param name="original"></param>
        /// <param name="item"></param>
        /// <param name="comparer"></param>
        /// <param name="index"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool BinarySearch<T>(this IList<T> original, T item, Func<T, T, int> comparer, out int index)
        {
            return BinarySearch(original, 0, original.Count - 1, item, comparer, out index);
        }

        public static bool BinarySearch<T>(this IList<T> original, int startIndex, int endIndex, T item, Func<T, T, int> comparer, out int index)
        {
            while (startIndex <= endIndex)
            {
                var mid = startIndex + (endIndex - startIndex) / 2;
                var dir = comparer(item, original[mid]);

                if (dir == 0)
                {
                    index = mid;
                    return true;
                }

                if (dir < 0)
                    endIndex = mid - 1;
                else
                    startIndex = mid + 1;
            }

            index = -1;
            return false;
        }

        public static bool BinarySearch<T>(this IList<T> original, T item, IComparer<T> comparer, out int index)
        {
            return BinarySearch(original, 0, original.Count - 1, item, comparer, out index);
        }

        public static bool BinarySearch<T>(this IList<T> original, int startIndex, int endIndex, T item, IComparer<T> comparer, out int index)
        {
            while (startIndex <= endIndex)
            {
                var mid = startIndex + (endIndex - startIndex) / 2;
                var dir = comparer.Compare(item, original[mid]);

                if (dir == 0)
                {
                    index = mid;
                    return true;
                }

                if (dir < 0)
                    endIndex = mid - 1;
                else
                    startIndex = mid + 1;
            }

            index = -1;
            return false;
        }

        public static unsafe bool BinarySerach<T>(T* original, int startIndex, int endIndex, T item, Func<T, T, int> comparer, out int index) where T : unmanaged
        {
            while (startIndex <= endIndex)
            {
                var mid = startIndex + (endIndex - startIndex) / 2;
                var dir = comparer(item, original[mid]);

                if (dir == 0)
                {
                    index = mid;
                    return true;
                }

                if (dir < 0)
                    endIndex = mid - 1;
                else
                    startIndex = mid + 1;
            }

            index = -1;
            return false;
        }
    }
}