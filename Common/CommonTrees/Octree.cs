using System;

namespace CZToolKit
{
    public enum OctreeNodeType
    {
        TopLeftFront = 0,           // 前    左   上
        TopRightFront,              // 前    右   上
        BottomLeftFront,            // 前    左   下
        BottomRightFront,           // 前    右   下
        TopLeftBack,                // 后    左   上
        TopRightBack,               // 后    右   上            
        BottomLeftBack,             // 后    左   下
        BottomRightBack             // 后    右   下
    }

    public interface IOctreeNode
    {
        int Type { get; }
        
        object UserData { get; set; }
    }

    public interface IOctreeNode<T> where T : IOctreeNode<T>
    {
        int Type { get; }

        object UserData { get; set; }
        
        T TopLeftFront { get; set; }
        
        T TopRightFront { get; set; }
        
        T BottomLeftFront { get; set; }
        
        T BottomRightFront { get; set; }
        
        T TopLeftBack { get; set; }
        
        T TopRightBack { get; set; }
        
        T BottomLeftBack { get; set; }
        
        T BottomRightBack { get; set; }
    }

    public class OctreeNode : IOctreeNode, IOctreeNode<OctreeNode>
    {
        private int type;
        public object userData;
        public OctreeNode parent;
        public OctreeNode[] children = new OctreeNode[8];

        public int Type
        {
            get { return type; }
        }

        public object UserData
        {
            get { return userData; }
            set { userData = value; }
        }
        
        public OctreeNode TopLeftFront
        {
            get { return children[0]; }
            set { children[0] = value; }
        }
        
        public OctreeNode TopRightFront
        {
            get { return children[1]; }
            set { children[1] = value; }
        }
        
        public OctreeNode BottomLeftFront
        {
            get { return children[2]; }
            set { children[2] = value; }
        }
        
        public OctreeNode BottomRightFront
        {
            get { return children[3]; }
            set { children[3] = value; }
        }
        
        public OctreeNode TopLeftBack
        {
            get { return children[4]; }
            set { children[4] = value; }
        }
        
        public OctreeNode TopRightBack
        {
            get { return children[5]; }
            set { children[5] = value; }
        }
        
        public OctreeNode BottomLeftBack
        {
            get { return children[6]; }
            set { children[6] = value; }
        }
        
        public OctreeNode BottomRightBack
        {
            get { return children[7]; }
            set { children[7] = value; }
        }

        public void SetChild(int nodeType, OctreeNode child)
        {
            if (child == null)
                return;
            
            child.parent = this;
            child.type = nodeType;
            children[nodeType] = child;
        }

        public OctreeNode GetOrCreateChild(int nodeType)
        {
            var child = children[nodeType];
            if (child == null)
            {
                child = new OctreeNode();
                SetChild(nodeType, child);
            }

            return child;
        }
        
        public void RemoveChild(OctreeNode child)
        {
            if (child == null)
                return;
            
            if (child != children[child.type])
                return;
            
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