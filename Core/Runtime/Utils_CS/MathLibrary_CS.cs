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
    public static partial class MathLibrary
    {
        /// <summary> 返回<paramref name="value"/>在<paramref name="a"/>和<paramref name="b"/>之间0-1的值 </summary>
        public static float ToLerp(float a, float b, float value)
        {
            return (value - a) / (b - a);
        }
    }
}
