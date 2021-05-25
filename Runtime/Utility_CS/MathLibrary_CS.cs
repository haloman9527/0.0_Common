#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 
 *
 */
#endregion

namespace CZToolKit.Core
{
    public static partial class MathLibrary
    {
        /// <summary> 返回<paramref name="_value"/>在<paramref name="_a"/>和<paramref name="_b"/>之间0-1的值 </summary>
        public static float ToLerp(float _a, float _b, float _value)
        {
            return (_value - _a) / (_b - _a);
        }
    }
}
