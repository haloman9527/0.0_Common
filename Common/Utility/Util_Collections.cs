using System;
using System.Collections.Generic;

namespace CZToolKit.Common
{
    public static class Util_Collections
    {
        public static bool TryGet<T>(this IList<T> array, int index, out T element)
        {
            element = default;
            if (array.Count > index)
            {
                element = array[index];
                return true;
            }

            return false;
        }
        
        public static int BinarySearch<T>(this IList<T> original, Func<T, int> comparer)
        {
            return BinarySearch(original, 0, original.Count - 1, comparer);
        }

        public static int BinarySearch<T>(this IList<T> original, int left, int right, Func<T, int> comparer)
        {
            while (left <= right)
            {
                var mid = (left + right) / 2;
                var dir = comparer(original[mid]);
                if (dir == 0)
                    return mid;
                if (dir < 0)
                    right = mid - 1;
                else
                    left = mid + 1;
            }
            return -1;
        }

        public static bool QuickSort<T>(this IList<T> original, Func<T, T, int> comparer)
        {
            if (original.Count <= 1)
                return false;
            return QuickSort(original, 0, original.Count - 1, comparer);
        }

        public static bool QuickSort<T>(this IList<T> original, int left, int right, Func<T, T, int> comparer)
        {
            if (left >= right)
                return false;
            T middle = original[left];
            int less = left;
            int greater = right;
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

            changed |= QuickSort(original, left, less, comparer);
            changed |= QuickSort(original, less + 1, right, comparer);
            return changed;
        }
    }
}