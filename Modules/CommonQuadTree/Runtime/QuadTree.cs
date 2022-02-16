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

namespace CZToolKit.Core.CommonQuadTree
{
    public interface IQuadTreeNode<T> where T : IQuadTreeNode<T>
    {
        T TopLeft { get; set; }
        T TopRight { get; set; }
        T BottomLeft { get; set; }
        T BottomRight { get; set; }
    }
}
