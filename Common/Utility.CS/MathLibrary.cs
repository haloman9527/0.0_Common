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

namespace CZToolKit.Common
{
    public static class MathLib
    {
        /// <summary> 返回<paramref name="value"/>在<paramref name="lhs"/>和<paramref name="rhs"/>之间0-1的值 </summary>
        public static float ToLerp(float lhs, float rhs, float value)
        {
            return (value - lhs) / (rhs - lhs);
        }
    }
}
