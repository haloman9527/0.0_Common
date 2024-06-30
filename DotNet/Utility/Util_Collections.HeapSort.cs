using System;
using System.Collections.Generic;

namespace CZToolKit
{
    public static partial class Util_Collections
    {
        /// <summary>
        /// 堆排序
        /// </summary>
        /// <param name="original"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns> 是否有变化 </returns>
        public static void HeapSort<T>(this IList<T> original, IComparer<T> comparer)
        {
            if (original.Count <= 1)
                return;
            HeapSort(original, 0, original.Count - 1, comparer);
        }

        /// <summary>
        /// 堆排序
        /// </summary>
        /// <param name="original"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="T"></typeparam>
        public static void HeapSort<T>(this IList<T> original, int startIndex, int endIndex, IComparer<T> comparer)
        {
            if (startIndex >= endIndex)
            {
                return;
            }

            // 1.建堆
            var length = endIndex - startIndex + 1;
            for (int i = ((length / 2) - (length % 2)); i >= 0; i--)
            {
                AdjustHeap(original, startIndex, endIndex, i, -1, comparer);
            }

            // 2.循环将堆首位（最大值）与末位交换，然后在重新调整最大堆
            while (endIndex > startIndex)
            {
                var temp = original[endIndex];
                original[endIndex] = original[startIndex];
                original[startIndex] = temp;

                endIndex--;
                AdjustHeap(original, startIndex, endIndex, 0, -1, comparer);
            }
        }

        private static void AdjustHeap<T>(IList<T> original, int startIndex, int endIndex, int i, int direction, IComparer<T> comparer)
        {
            var length = endIndex - startIndex + 1;

            while (true)
            {
                var maxIndex = i;

                if (i * 2 + 1 < length && comparer.Compare(original[startIndex + maxIndex], original[startIndex + (i * 2 + 1)]) * direction > 0)
                {
                    maxIndex = i * 2 + 1;
                }

                if (i * 2 + 2 < length && comparer.Compare(original[startIndex + maxIndex], original[startIndex + (i * 2 + 2)]) * direction > 0)
                {
                    maxIndex = i * 2 + 2;
                }

                if (maxIndex == i)
                {
                    break;
                }

                var temp = original[startIndex + maxIndex];
                original[startIndex + maxIndex] = original[startIndex + i];
                original[startIndex + i] = temp;
                i = maxIndex;
            }
        }

        /// <summary>
        /// 堆排序
        /// </summary>
        /// <param name="original"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns> 是否有变化 </returns>
        public static void HeapSort<T>(this IList<T> original, Func<T, T, int> comparer)
        {
            if (original.Count <= 1)
                return;
            HeapSort(original, 0, original.Count - 1, comparer);
        }

        /// <summary>
        /// 堆排序
        /// </summary>
        /// <param name="original"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns> 是否有变化 </returns>
        public static void HeapSort<T>(this IList<T> original, int startIndex, int endIndex, Func<T, T, int> comparer)
        {
            if (startIndex >= endIndex)
            {
                return;
            }

            // 1.建堆
            var length = endIndex - startIndex + 1;
            for (int i = ((length / 2) - (length % 2)); i >= 0; i--)
            {
                AdjustHeap(original, startIndex, endIndex, i, -1, comparer);
            }

            // 2.循环将堆首位（最大值）与末位交换，然后在重新调整最大堆
            while (endIndex > startIndex)
            {
                var temp = original[endIndex];
                original[endIndex] = original[startIndex];
                original[startIndex] = temp;

                endIndex--;
                AdjustHeap(original, startIndex, endIndex, 0, -1, comparer);
            }
        }

        /// <summary>
        /// 下沉调整
        /// </summary>
        /// <param name="original"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="i"></param>
        /// <param name="direction"> </param>
        /// <param name="comparer"></param>
        /// <typeparam name="T"></typeparam>
        private static void AdjustHeap<T>(IList<T> original, int startIndex, int endIndex, int i, int direction, Func<T, T, int> comparer)
        {
            var length = endIndex - startIndex + 1;

            while (true)
            {
                var maxIndex = i;

                if (i * 2 + 1 < length && comparer(original[startIndex + maxIndex], original[startIndex + (i * 2 + 1)]) * direction > 0)
                {
                    maxIndex = i * 2 + 1;
                }

                if (i * 2 + 2 < length && comparer(original[startIndex + maxIndex], original[startIndex + (i * 2 + 2)]) * direction > 0)
                {
                    maxIndex = i * 2 + 2;
                }

                if (maxIndex == i)
                {
                    break;
                }

                var temp = original[startIndex + maxIndex];
                original[startIndex + maxIndex] = original[startIndex + i];
                original[startIndex + i] = temp;
                i = maxIndex;
            }
        }

        public static unsafe void HeapSort<T>(T* original, int startIndex, int endIndex, IComparer<T> comparer) where T : unmanaged
        {
            if (startIndex >= endIndex)
            {
                return;
            }

            // 1.建堆
            var length = endIndex - startIndex + 1;
            for (int i = ((length / 2) - (length % 2)); i >= 0; i--)
            {
                AdjustHeap(original, startIndex, endIndex, i, -1, comparer);
            }

            // 2.循环将堆首位（最大值）与末位交换，然后在重新调整最大堆
            while (endIndex > startIndex)
            {
                var temp = original[endIndex];
                original[endIndex] = original[startIndex];
                original[startIndex] = temp;

                endIndex--;
                AdjustHeap(original, startIndex, endIndex, 0, -1, comparer);
            }
        }

        /// <summary>
        /// 下沉调整
        /// </summary>
        /// <param name="original"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="i"></param>
        /// <param name="direction"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="T"></typeparam>
        private static unsafe void AdjustHeap<T>(T* original, int startIndex, int endIndex, int i, int direction, IComparer<T> comparer) where T : unmanaged
        {
            var length = endIndex - startIndex + 1;

            while (true)
            {
                var maxIndex = i;

                if (i * 2 + 1 < length && comparer.Compare(original[startIndex + maxIndex], original[startIndex + (i * 2 + 1)]) * direction > 0)
                {
                    maxIndex = i * 2 + 1;
                }

                if (i * 2 + 2 < length && comparer.Compare(original[startIndex + maxIndex], original[startIndex + (i * 2 + 2)]) * direction > 0)
                {
                    maxIndex = i * 2 + 2;
                }

                if (maxIndex == i)
                {
                    break;
                }

                var temp = original[startIndex + maxIndex];
                original[startIndex + maxIndex] = original[startIndex + i];
                original[startIndex + i] = temp;
                i = maxIndex;
            }
        }

        public static unsafe void HeapSort<T>(T* original, int startIndex, int endIndex, Func<T, T, int> comparer) where T : unmanaged
        {
            if (startIndex >= endIndex)
            {
                return;
            }

            // 1.建堆
            var length = endIndex - startIndex + 1;
            for (int i = ((length / 2) - (length % 2)); i >= 0; i--)
            {
                AdjustHeap(original, startIndex, endIndex, i, -1, comparer);
            }

            // 2.循环将堆首位（最大值）与末位交换，然后在重新调整最大堆
            while (endIndex > startIndex)
            {
                var temp = original[endIndex];
                original[endIndex] = original[startIndex];
                original[startIndex] = temp;

                endIndex--;
                AdjustHeap(original, startIndex, endIndex, 0, -1, comparer);
            }
        }

        /// <summary>
        /// 下沉调整
        /// </summary>
        /// <param name="original"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="i"></param>
        /// <param name="direction"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="T"></typeparam>
        private static unsafe void AdjustHeap<T>(T* original, int startIndex, int endIndex, int i, int direction, Func<T, T, int> comparer) where T : unmanaged
        {
            var length = endIndex - startIndex + 1;

            while (true)
            {
                var maxIndex = i;

                if (i * 2 + 1 < length && comparer(original[startIndex + maxIndex], original[startIndex + (i * 2 + 1)]) * direction > 0)
                {
                    maxIndex = i * 2 + 1;
                }

                if (i * 2 + 2 < length && comparer(original[startIndex + maxIndex], original[startIndex + (i * 2 + 2)]) * direction > 0)
                {
                    maxIndex = i * 2 + 2;
                }

                if (maxIndex == i)
                {
                    break;
                }

                var temp = original[startIndex + maxIndex];
                original[startIndex + maxIndex] = original[startIndex + i];
                original[startIndex + i] = temp;
                i = maxIndex;
            }
        }
    }
}