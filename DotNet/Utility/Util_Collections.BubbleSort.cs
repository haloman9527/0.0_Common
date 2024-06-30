using System;
using System.Collections.Generic;

namespace CZToolKit
{
    public static partial class Util_Collections
    {
        /// <summary>
        /// 冒泡排序
        /// </summary>
        /// <param name="original"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns> 是否有变化 </returns>
        public static bool BubbleSort<T>(this IList<T> original, IComparer<T> comparer)
        {
            if (original.Count <= 1)
                return false;
            return BubbleSort(original, 0, original.Count - 1, comparer);
        }

        /// <summary>
        /// 冒泡排序
        /// </summary>
        /// <param name="original"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns> 是否有变化 </returns>
        public static bool BubbleSort<T>(this IList<T> original, int startIndex, int endIndex, IComparer<T> comparer)
        {
            var changed = false;
            while (endIndex > startIndex)
            {
                var lastExchangeIndex = endIndex;
                var exchanged = false;
                for (int i = startIndex + 1; i <= endIndex; i++)
                {
                    var pre = original[i - 1];
                    var cur = original[i];
                    if (comparer.Compare(pre, cur) > 0)
                    {
                        original[i] = pre;
                        original[i - 1] = cur;
                        exchanged = true;
                        lastExchangeIndex = i;
                    }
                }

                changed |= exchanged;
                if (!exchanged)
                {
                    break;
                }

                endIndex = lastExchangeIndex;
            }

            return changed;
        }

        /// <summary>
        /// 冒泡排序
        /// </summary>
        /// <param name="original"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns> 是否有变化 </returns>
        public static bool BubbleSort<T>(this IList<T> original, Func<T, T, int> comparer)
        {
            if (original.Count <= 1)
                return false;
            return BubbleSort(original, 0, original.Count - 1, comparer);
        }

        /// <summary>
        /// 冒泡排序
        /// </summary>
        /// <param name="original"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns> 是否有变化 </returns>
        public static bool BubbleSort<T>(this IList<T> original, int startIndex, int endIndex, Func<T, T, int> comparer)
        {
            var changed = false;
            while (endIndex > startIndex)
            {
                var lastExchangeIndex = endIndex;
                var exchanged = false;
                for (int i = startIndex + 1; i <= endIndex; i++)
                {
                    var pre = original[i - 1];
                    var cur = original[i];
                    if (comparer(pre, cur) > 0)
                    {
                        original[i] = pre;
                        original[i - 1] = cur;
                        exchanged = true;
                        lastExchangeIndex = i;
                    }
                }

                changed |= exchanged;
                if (!exchanged)
                {
                    break;
                }

                endIndex = lastExchangeIndex;
            }
            
            return changed;
        }

        public static unsafe bool BubbleSort<T>(T* original, int startIndex, int endIndex, IComparer<T> comparer) where T : unmanaged
        {
            var changed = false;
            while (endIndex > startIndex)
            {
                var lastExchangeIndex = endIndex;
                var exchanged = false;
                for (int i = startIndex + 1; i <= endIndex; i++)
                {
                    var pre = original[i - 1];
                    var cur = original[i];
                    if (comparer.Compare(pre, cur) > 0)
                    {
                        original[i] = pre;
                        original[i - 1] = cur;
                        exchanged = true;
                        lastExchangeIndex = i;
                    }
                }

                changed |= exchanged;
                if (!exchanged)
                {
                    break;
                }

                endIndex = lastExchangeIndex;
            }
            
            return changed;
        }

        public static unsafe bool BubbleSort<T>(T* original, int startIndex, int endIndex, Func<T, T, int> comparer) where T : unmanaged
        {
            var changed = false;
            while (endIndex > startIndex)
            {
                var lastExchangeIndex = endIndex;
                var exchanged = false;
                for (int i = startIndex + 1; i <= endIndex; i++)
                {
                    var pre = original[i - 1];
                    var cur = original[i];
                    if (comparer(pre, cur) > 0)
                    {
                        original[i] = pre;
                        original[i - 1] = cur;
                        exchanged = true;
                        lastExchangeIndex = i;
                    }
                }

                changed |= exchanged;
                if (!exchanged)
                {
                    break;
                }

                endIndex = lastExchangeIndex;
            }
            
            return changed;
        }
    }
}