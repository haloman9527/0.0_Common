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
    public interface IBinaryTree<T>
    {
        INode<T> Root { get; set; }
        Func<T, T, Direction> Role { get; }

        bool Insert(T userData);
        bool Insert(INode<T> node);
    }

    public interface INode<T>
    {
        T UserData { get; }
        INode<T> Left { get; set; }
        INode<T> Right { get; set; }

        bool Insert(T userData);
        bool Insert(INode<T> node);
        int GetHeight();
    }

    /// <summary>
    /// TODO: 非平衡二叉树，后续增加扩展
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BinaryTree<T> : IBinaryTree<T>
    {
        public INode<T> Root { get; set; }
        public Func<T, T, Direction> Role { get; private set; }

        public BinaryTree(Func<T, T, Direction> role)
        {
            Role = role;
        }

        public bool Insert(T userData)
        {
            return Insert(new Node<T>(userData, Role));
        }

        public bool Insert(INode<T> node)
        {
            if (Root == null)
            {
                Root = node;
                return true;
            }
            return Root.Insert(node);
        }
    }

    public class Node<T> : INode<T>
    {
        public T UserData { get; set; }
        public INode<T> Left { get; set; }
        public INode<T> Right { get; set; }
        public Func<T, T, Direction> Role { get; private set; }

        public Node(T userData, Func<T, T, Direction> role)
        {
            this.UserData = userData;
            this.Role = role;
        }

        public bool Insert(T userData)
        {
            return Insert(new Node<T>(userData, Role));
        }

        public bool Insert(INode<T> node)
        {
            return Insert(node, this);
        }

        // 递归插入
        bool Insert(INode<T> node, INode<T> parent)
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

        public int GetHeight()
        {
            return 0;
        }
    }

    public enum Direction
    {
        Left, Right, Failure
    }

    public static class BinaryTreeExtension
    {
        /// <summary> 前序遍历 </summary>
        public static IEnumerable<T> PreorderEnumerate<T>(this IBinaryTree<T> tree)
        {
            foreach (var data in PreorderEnumerate(tree.Root))
            {
                yield return data;
            }
        }

        /// <summary> 中序遍历 </summary>
        public static IEnumerable<T> InorderEnumerate<T>(this IBinaryTree<T> tree)
        {
            foreach (var data in InorderEnumerate(tree.Root))
            {
                yield return data;
            }
        }

        /// <summary> 后序遍历 </summary>
        public static IEnumerable<T> PostorderEnumerate<T>(this IBinaryTree<T> tree)
        {
            foreach (var data in PostorderEnumerate(tree.Root))
            {
                yield return data;
            }
        }

        /// <summary> 前序遍历 </summary>
        public static IEnumerable<T> PreorderEnumerate<T>(this INode<T> root)
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
        public static IEnumerable<T> InorderEnumerate<T>(this INode<T> root)
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
        public static IEnumerable<T> PostorderEnumerate<T>(this INode<T> root)
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
    }
}
