using System;

namespace Moyo
{
    public class FullBinaryTreeNode
    {
        
        private int type;
        private object userData;
        public FullBinaryTreeNode parent;
        public FullBinaryTreeNode[] children = new FullBinaryTreeNode[2];

        public int Type
        {
            get { return type; }
        }
        
        public object UserData
        {
            get { return userData; }
            set { userData = value; }
        }

        public FullBinaryTreeNode Left
        {
            get { return children[0]; }
            set { children[0] = value; }
        }

        public FullBinaryTreeNode Right
        {
            get { return children[1]; }
            set { children[1] = value; }
        }

        public void SetChild(int nodeType, FullBinaryTreeNode child)
        {
            if (child == null)
                throw new NullReferenceException();

            child.parent = this;
            child.type = nodeType;
            children[nodeType] = child;
        }

        public FullBinaryTreeNode GetOrCreateChild(int nodeType)
        {
            var child = children[nodeType];
            if (child == null)
            {
                child = new FullBinaryTreeNode();
                SetChild(nodeType, child);
            }

            return child;
        }
        
        public void RemoveChild(FullBinaryTreeNode child)
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
}