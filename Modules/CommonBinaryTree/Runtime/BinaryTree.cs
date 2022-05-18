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
using System.Collections.Generic;

namespace CZToolKit.Core.CommonBinaryTree
{
    public interface IBinaryTreeNode<T> where T : IBinaryTreeNode<T>
    {
        T Left { get; set; }
        T Right { get; set; }
    }

    public static class BinaryTreeExtension
    {
        /// <summary> 前序遍历 </summary>
        public static IEnumerable<T> PreorderEnumerate<T>(this T root) where T : IBinaryTreeNode<T>
        {
            if (root != null)
            {
                yield return root;
            }
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
        public static IEnumerable<T> InorderEnumerate<T>(this T root) where T : IBinaryTreeNode<T>
        {
            if (root.Left != null)
            {
                foreach (var data in InorderEnumerate(root.Left))
                {
                    yield return data;
                }
            }
            if (root != null)
            {
                yield return root;
            }
            if (root.Right != null)
            {
                foreach (var data in InorderEnumerate(root.Right))
                {
                    yield return data;
                }
            }
        }

        /// <summary> 后序遍历 </summary>
        public static IEnumerable<T> PostorderEnumerate<T>(this T root) where T : IBinaryTreeNode<T>
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
            if (root != null)
            {
                yield return root;
            }
        }

        /// <summary> 层次遍历 </summary>
        public static void LayerEnumerate<T>(this T root, ref Queue<T> queue) where T : IBinaryTreeNode<T>
        {
            queue.Clear();
            if (root != null)
            {
                queue.Enqueue(root);
            }
            while (queue.Count != 0)
            {
                var node = queue.Dequeue();
                if (node.Left != null)
                {
                    queue.Enqueue(node.Left);
                }
                if (node.Right != null)
                {
                    queue.Enqueue(node.Right);
                }
            }
        }
    }
}
