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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CZToolKit.Core
{
    public static partial class Utility
    {
        public static IEnumerable<Transform> EnumerateTransform(Transform root)
        {
            yield return root;
            for (int i = 0; i < root.childCount; i++)
            {
                foreach (var item in EnumerateTransform(root.GetChild(i)))
                {
                    yield return item;
                }
            }
        }
    }
}
