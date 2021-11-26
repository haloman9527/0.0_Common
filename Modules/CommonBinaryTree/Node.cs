#region 注 释
/***
 *
 *  Title:
 *      主题: 通用有序二叉树
 *  Description:
 *      功能:
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

namespace CZToolKit.Core.CommonBinaryTree
{
    public interface INode<T>
    {
        T UserData { get; }
    }

    public class Node<T> : INode<T>
    {
        public Node<T> left;
        public Node<T> right;

        public T UserData { get; private set; }

        public Node()
        {

        }


        public bool Insert(Node<T> node)
        {
            return false;
        }
    }
}
