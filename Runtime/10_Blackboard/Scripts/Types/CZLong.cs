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
    public class CZLong : CZType<long>
    {
        public CZLong() : base()
        { Value = 0; }

        public CZLong(long _value) : base(_value) { }

        public static implicit operator CZLong(long _other) { return new CZLong(_other); }
    }
}
