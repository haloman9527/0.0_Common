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

    public class OctreeNode : IDisposable
    {
        public object userData;
        public OctreeNodeType type;
        public OctreeNode parent;
        public OctreeNode[] children = new OctreeNode[8];

        public OctreeNode GetChild(OctreeNodeType nodeType)
        {
            var index = (int)nodeType;
            var child = children[index];
            if (child == null)
            {
                child = new OctreeNode();
                child.parent = this;
                child.type = nodeType;
                children[index] = child;
            }

            return child;
        }

        public void Dispose()
        {
            for (int i = 0; i < children.Length; i++)
            {
                var child = children[i];
                if (child == null)
                    continue;
                child.Dispose();
            }

            userData = null;
            if (parent != null)
                parent.children[(int)type] = null;
        }
    }
}