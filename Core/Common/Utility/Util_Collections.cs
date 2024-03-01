using System;
using System.Collections.Generic;

namespace CZToolKit
{
    public static class Util_Collections
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
                return 0;

            var mid = -1;
            while (startIndex <= endIndex)
            {
                mid = (startIndex + endIndex) / 2;
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

            return mid;
        }


        public static int BinaryMatch<T>(this IList<T> original, T item, IComparer<T> comparer)
        {
            return BinaryMatch(original, 0, original.Count - 1, item, comparer);
        }

        public static int BinaryMatch<T>(this IList<T> original, int startIndex, int endIndex, T item, IComparer<T> comparer)
        {
            if (original.Count == 0)
                return 0;

            var mid = -1;
            while (startIndex <= endIndex)
            {
                mid = (startIndex + endIndex) / 2;
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

            return mid;
        }

        /// <summary>
        /// 二分查找，返回item在数组中的索引
        /// </summary>
        /// <param name="original"></param>
        /// <param name="item"></param>
        /// <param name="comparer"></param>
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
                var mid = (startIndex + endIndex) / 2;
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
                var mid = (startIndex + endIndex) / 2;
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

        /// <summary>
        /// 快速排序
        /// </summary>
        /// <param name="original"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool QuickSort<T>(this IList<T> original, IComparer<T> comparer)
        {
            if (original.Count <= 1)
                return false;
            return QuickSort(original, 0, original.Count - 1, comparer);
        }

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
        /// <returns></returns>
        public static bool QuickSort<T>(this IList<T> original, Func<T, T, int> comparer)
        {
            if (original.Count <= 1)
                return false;
            return QuickSort(original, 0, original.Count - 1, comparer);
        }

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

        public static unsafe bool BinarySerach<T>(T* original, int startIndex, int endIndex, T item, Func<T, T, int> comparer, out int index) where T : unmanaged
        {
            while (startIndex <= endIndex)
            {
                var mid = (startIndex + endIndex) / 2;
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
    }
}