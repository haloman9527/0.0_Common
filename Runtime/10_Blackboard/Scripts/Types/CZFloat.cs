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
    public class CZFloat : CZType<float>
    {
        public CZFloat() : base()
        { Value = 0; }

        public CZFloat(float _value) : base(_value) { }

        public static implicit operator CZFloat(float _other) { return new CZFloat(_other); }
    }
}
