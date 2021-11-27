#region 注 释
/***
 *
 *  Title:
 *      主题: 通用二叉树
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
using System;
using System.Collections.Generic;

namespace CZToolKit.Core.CommonBinaryTree
{
    public class BinaryTree
    {
        #region 静态
        /// <summary> 前序遍历 </summary>
        public static IEnumerable<T> PreorderEnumerate<T>(BinaryTree<T> tree)
        {
            foreach (var data in PreorderEnumerate(tree.Root))
            {
                yield return data;
            }
        }

        /// <summary> 中序遍历 </summary>
        public static IEnumerable<T> InorderEnumerate<T>(BinaryTree<T> tree)
        {
            foreach (var data in InorderEnumerate(tree.Root))
            {
                yield return data;
            }
        }

        /// <summary> 后序遍历 </summary>
        public static IEnumerable<T> PostorderEnumerate<T>(BinaryTree<T> tree)
        {
            foreach (var data in PostorderEnumerate(tree.Root))
            {
                yield return data;
            }
        }

        /// <summary> 前序遍历 </summary>
        public static IEnumerable<T> PreorderEnumerate<T>(Node<T> root)
        {
            yield return root.UserData;
            if (root.Left != null)
            {
                foreach (var data in PreorderEnumerate(root.Left))
                {
                    yield return data;
                }
            }
            if (root.Right != null)
            {
                foreach (var data in PreorderEnumerate(root.Right))
                {
                    yield return data;
                }
            }
        }

        /// <summary> 中序遍历 </summary>
        public static IEnumerable<T> InorderEnumerate<T>(Node<T> root)
        {
            if (root.Left != null)
            {
                foreach (var data in InorderEnumerate(root.Left))
                {
                    yield return data;
                }
            }
            yield return root.UserData;
            if (root.Right != null)
            {
                foreach (var data in InorderEnumerate(root.Right))
                {
                    yield return data;
                }
            }
        }

        /// <summary> 中序遍历 </summary>
        public static IEnumerable<T> PostorderEnumerate<T>(Node<T> root)
        {
            if (root.Left != null)
            {
                foreach (var data in PostorderEnumerate(root.Left))
                {
                    yield return data;
                }
            }
            if (root.Right != null)
            {
                foreach (var data in PostorderEnumerate(root.Right))
                {
                    yield return data;
                }
            }
            yield return root.UserData;
        }
        #endregion
    }

    /// <summary>
    /// TODO: 非平衡二叉树，后续增加扩展
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BinaryTree<T> : BinaryTree
    {
        public Node<T> Root { get; set; }
        Func<T, T, Direction> Role { get; set; }

        public BinaryTree(Func<T, T, Direction> role)
        {
            Role = role;
        }

        public bool Insert(T userData)
        {
            Node<T> node = new Node<T>(userData);
            return Insert(node);
        }

        public bool Insert(Node<T> node)
        {
            if (Root == null)
            {
                Root = node;
                return true;
            }
            return Insert(node, Root);
        }

        // 递归插入
        bool Insert(Node<T> node, Node<T> parent)
        {
            Direction dir = Role(node.UserData, parent.UserData);

            switch (dir)
            {
                case Direction.Left:
                    if (parent.Left == null)
                    {
                        parent.Left = node;
                        break;
                    }
                    Insert(node, parent.Left);
                    break;
                case Direction.Right:
                    if (parent.Right == null)
                    {
                        parent.Right = node;
                        break;
                    }
                    Insert(node, parent.Right);
                    break;
                case Direction.Failure:
                    return false;
            }
            return true;
        }

    }

    public class Node<T>
    {
        public T UserData { get; set; }
        public Node<T> Left { get; set; }
        public Node<T> Right { get; set; }

        public Node(T userData)
        {
            this.UserData = userData;
        }
    }

    public enum Direction
    {
        Left, Right, Failure
    }
}
