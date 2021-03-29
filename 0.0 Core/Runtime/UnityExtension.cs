using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class UnityExtension
{
    /// <summary> 获取CC的真实高度 </summary>
    public static float GetRealHeight(this CharacterController characterController)
    {
        return Mathf.Max(characterController.radius * 2, characterController.height);
    }

    /// <summary> 获取CC顶部半圆中心 </summary>
    public static Vector3 GetTopCenter(this CharacterController characterController)
    {
        return Vector3.down * characterController.radius + Vector3.up * characterController.GetRealHeight() / 2 + characterController.center;
    }

    /// <summary> 获取CC底部半圆中心 </summary>
    public static Vector3 GetBottomCenter(this CharacterController characterController)
    {
        return Vector3.up * characterController.radius + Vector3.down * characterController.GetRealHeight() / 2 + characterController.center;
    }

    /// <summary> 快速排序 </summary>
    public static void Sort<T>(this List<T> _original, Func<T, T, bool> _func)
    {
        if (_original.Count == 1)
            return;

        // 抽取一个数据作为中间值
        int index = UnityEngine.Random.Range(0, _original.Count);
        T rN = _original[index];

        // 声明小于中间值的列表
        List<T> left = new List<T>(Mathf.Max(4, _original.Count / 2));
        // 声明大于中间值的列表
        List<T> right = new List<T>(Mathf.Max(4, _original.Count / 2));
        // 遍历数组，与中间值比较，小于中间值的放在left，大于中间值的放在right
        for (int i = 0; i < _original.Count; i++)
        {
            if (i == index) continue;

            if (_func(_original[i], rN))
                left.Add(_original[i]);
            else
                right.Add(_original[i]);
        }

        _original.Clear();

        // 如果左列表元素个数不为0，就把左列表也排序
        if (left.Count != 0)
        {
            left.Sort(_func);
            _original.AddRange(left);
        }
        _original.Add(rN);
        // 如果右列表元素个数不为0，就把右列表也排序
        if (right.Count != 0)
        {
            right.Sort(_func);
            _original.AddRange(right);
        }
        return;
    }

    /// <summary> 快速排序 </summary>
    //public static IEnumerable<T> Sort<T>(this IEnumerable<T> _original, Func<T, T, bool> _func)
    //{
    //    int count = _original.Count();
    //    if (count == 1)
    //        yield return _original.ElementAt(0);

    //    // 抽取一个数据作为中间值
    //    int index = UnityEngine.Random.Range(0, count);
    //    T rN = _original.ElementAt(index);

    //    // 声明小于中间值的列表
    //    List<T> left = new List<T>(Mathf.Max(4, count / 2));
    //    // 声明大于中间值的列表
    //    List<T> right = new List<T>(Mathf.Max(4, count / 2));

    //    int i = 0;
    //    foreach (var item in _original)
    //    {
    //        if (i == index) continue;
    //        if (_func(item, rN))
    //            left.Add(item);
    //        else
    //            right.Add(item);
    //        i++;
    //    }

    //    // 如果左列表元素个数不为0，就把左列表也排序
    //    if (left.Count != 0)
    //    {
    //        foreach (var item in left.Sort(_func))
    //        {
    //            Debug.Log(item);
    //            yield return item;
    //        }
    //    }

    //    yield return rN;

    //    // 如果右列表元素个数不为0，就把右列表也排序
    //    if (right.Count != 0)
    //    {
    //        foreach (var item in right.Sort(_func))
    //        {
    //            yield return item;
    //        }
    //    }
    //}
}
