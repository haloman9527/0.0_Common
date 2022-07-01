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
    public static bool TryGet<T>(this T[] array, int index, out T element)
    {
        element = default;
        if (array.Length > index)
        {
            element = array[index];
            return true;
        }
        return false;
    }

    /// <summary> 快速排序(第二个参数是中间值) </summary>
    public static void QuickSort<T>(this IList<T> original, Func<T, T, int> comparer)
    {
        if (original.Count <= 1)
            return;
        QuickSort(0, original.Count - 1);
        void QuickSort(int left, int right)
        {
            if (left < right)
            {
                int middleIndex = (left + right) / 2;
                T middle = original[middleIndex];
                int i = left;
                int j = right;
                while (true)
                {
                    // 双指针收缩
                    // 找到一个大于中数的下标和一个小于中数的下标，交换位置
                    while (i < j && comparer(original[i], middle) < 0) { i++; };
                    while (j > i && comparer(original[j], middle) > 0) { j--; };
                    if (i == j) break;

                    T temp = original[i];
                    original[i] = original[j];
                    original[j] = temp;

                    if (comparer(original[i], original[j]) == 0) j--;
                }

                QuickSort(left, i);
                QuickSort(i + 1, right);
            }
        }
    }
}
