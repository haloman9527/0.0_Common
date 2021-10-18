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
using System;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZInt : CZType<int>
    {
        public CZInt() : base()
        { Value = 0; }

        public CZInt(int _value) : base(_value) { }

        public static implicit operator CZInt(int _other) { return new CZInt(_other); }
    }
}
