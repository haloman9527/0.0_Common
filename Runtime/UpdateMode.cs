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
    public enum UpdateMode
    {
        /// <summary> Update </summary>
        Normal = 0,
        /// <summary> FixedUpdate </summary>
        AnimatePhysics = 1,
        /// <summary> UnScaledUpdate </summary>
        UnscaledTime = 2,
        Manual = 3
    }
}