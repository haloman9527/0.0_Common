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
 *  Blog: https://www.mindgear.net/
 *
 */

#endregion

using System;
using System.Collections.Generic;

namespace CZToolKit
{
    public enum BinaryTreeNodeType
    {
        Left = 0,           // 左
        Right,              // 右
    }
    
    public interface IBindaryTreeNode
    {
        int Type { get; }

        object UserData { get; set; }
    }

    public interface IBinaryTreeNode<T> where T : IBinaryTreeNode<T>
    {
        int Type { get; }

        object UserData { get; set; }

        T Left { get; set; }

        T Right { get; set; }
    }

    public class BinaryTreeNode : IBindaryTreeNode, IBinaryTreeNode<BinaryTreeNode>
    {
        private int type;
        private object userData;
        public BinaryTreeNode parent;
        public BinaryTreeNode[] children = new BinaryTreeNode[2];

        public int Type
        {
            get { return type; }
        }
        
        public object UserData
        {
            get { return userData; }
            set { userData = value; }
        }

        public BinaryTreeNode Left
        {
            get { return children[0]; }
            set { children[0] = value; }
        }

        public BinaryTreeNode Right
        {
            get { return children[1]; }
            set { children[1] = value; }
        }

        public void SetChild(int nodeType, BinaryTreeNode child)
        {
            if (child == null)
                throw new NullReferenceException();

            child.parent = this;
            child.type = nodeType;
            children[nodeType] = child;
        }

        public BinaryTreeNode GetOrCreateChild(int nodeType)
        {
            var child = children[nodeType];
            if (child == null)
            {
                child = new BinaryTreeNode();
                SetChild(nodeType, child);
            }

            return child;
        }
        
        public void RemoveChild(BinaryTreeNode child)
        {
            if (child == null)
                throw new NullReferenceException();
            
            if (child.parent != this)
                throw new InvalidOperationException();
            
            if (child != children[child.type])
                throw new InvalidOperationException();
            
            child.parent = null;
            children[child.type] = null;
        }
        
        public void RemoveChild(int nodeType)
        {
            var child = children[nodeType];
            RemoveChild(child);
        }
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
                foreach (var node in PreorderEnumerate(root.Left))
                {
                    yield return node;
                }
            }

            if (root.Right != null)
            {
                foreach (var node in PreorderEnumerate(root.Right))
                {
                    yield return node;
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