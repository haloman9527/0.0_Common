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

namespace CZToolKit.Core
{
    public enum UIDirection
    {
        None = 0,
        Left = 1 << 0,
        Right = 1 << 1,
        Top = 1 << 2,
        Bottom = 1 << 3,
        TopLeft = 1 << 4,
        TopRight = 1 << 5,
        MiddleCenter = 1 << 6,
        BottomLeft = 1 << 7,
        BottomRight = 1 << 8,
    }
}
