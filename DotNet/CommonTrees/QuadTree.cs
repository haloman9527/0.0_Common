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
 *  Github: https://github.com/haloman9527
 *  Blog: https://www.haloman.net/
 *
 */

#endregion

using System;

namespace CZToolKit
{
    public enum QuadTreeNodeType
    {
        LeftFront = 0,      // 前    左
        RightFront,         // 前    右
        LeftBack,           // 后    左
        RightBack,          // 后    右
    }

    public interface IQuadTreeNode
    {
        int Type { get; }
        
        object UserData { get; set; }
    }

    public interface IQuadTreeNode<T> where T : IQuadTreeNode<T>
    {
        int Type { get; }

        object UserData { get; set; }

        T LeftFront { get; set; }

        T RightFront { get; set; }

        T LeftBack { get; set; }

        T RightBack { get; set; }
    }

    public class QuadTreeNode : IQuadTreeNode, IQuadTreeNode<QuadTreeNode>
    {
        private int type;
        private object userData;
        public QuadTreeNode parent;
        public QuadTreeNode[] children = new QuadTreeNode[4];

        public int Type
        {
            get { return type; }
        }

        public object UserData
        {
            get { return userData; }
            set { userData = value; }
        }

        public QuadTreeNode LeftFront
        {
            get { return children[0]; }
            set { children[0] = value; }
        }

        public QuadTreeNode RightFront
        {
            get { return children[1]; }
            set { children[1] = value; }
        }

        public QuadTreeNode LeftBack
        {
            get { return children[2]; }
            set { children[2] = value; }
        }

        public QuadTreeNode RightBack
        {
            get { return children[3]; }
            set { children[3] = value; }
        }

        public void SetChild(int nodeType, QuadTreeNode child)
        {
            if (child == null)
                throw new InvalidOperationException();
            
            child.parent = this;
            child.type = nodeType;
            children[nodeType] = child;
        }

        public QuadTreeNode GetOrCreateChild(int nodeType)
        {
            var child = children[nodeType];
            if (child == null)
            {
                child = new QuadTreeNode();
                SetChild(nodeType, child);
            }

            return child;
        }
        
        public void RemoveChild(QuadTreeNode child)
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
            RemoveChild(children[nodeType]);
        }
    }
}