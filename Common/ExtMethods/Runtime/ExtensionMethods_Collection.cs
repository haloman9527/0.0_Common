#region 注 释

/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 半只龙虾人
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */

#endregion

using System;
using System.Collections.Generic;

public static partial class ExtMethods
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

    /// <returns> List Changed </returns>
    public static bool QuickSort<T>(this IList<T> original, Func<T, T, int> comparer)
    {
        if (original.Count <= 1)
            return false;
        return QuickSort(0, original.Count - 1);

        bool QuickSort(int left, int right)
        {
            if (left >= right)
                return false;
            int middleIndex = (left + right) / 2;
            T middle = original[middleIndex];
            int i = left;
            int j = right;
            bool changed = false;
            while (true)
            {
                // 双指针收缩
                // 找到一个大于中数的下标和一个小于中数的下标，交换位置
                while (i < j && comparer(original[i], middle) < 0)
                {
                    i++;
                }

                while (j > i && comparer(original[j], middle) > 0)
                {
                    j--;
                }

                if (i == j) break;

                T temp = original[i];
                original[i] = original[j];
                original[j] = temp;
                changed = true;
                if (comparer(original[i], original[j]) == 0) j--;
            }

            changed |= QuickSort(left, i);
            changed |= QuickSort(i + 1, right);
            return changed;
        }
    }
}