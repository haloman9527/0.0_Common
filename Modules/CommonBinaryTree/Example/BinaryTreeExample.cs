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
using UnityEngine;
using CZToolKit.Core.CommonBinaryTree;

public class BinaryTreeExample : MonoBehaviour
{
    void Test()
    {
        BinaryTree<int> tree = new BinaryTree<int>((a, b) =>
        {
            if (a <= b)
            {
                return Direction.Left;
            }
            return Direction.Right;
        });

        tree.Insert(0);
        tree.Insert(5);
        tree.Insert(1);
        tree.Insert(7);
        tree.Insert(3);
        tree.Insert(4);
        tree.Insert(2);
        tree.Insert(6);
        tree.Insert(8);

        foreach (var item in tree.InorderEnumerate())
        {
            Debug.Log(item);
        }
    }
}
