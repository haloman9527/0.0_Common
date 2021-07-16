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
    public class CZString : CZType<string>
    {
        public CZString() : base()
        { Value = ""; }

        public CZString(string _value) : base(_value) { }

        public static implicit operator CZString(string _other) { return new CZString(_other); }
    }
}
