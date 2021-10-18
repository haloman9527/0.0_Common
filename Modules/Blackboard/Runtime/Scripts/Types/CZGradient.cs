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
using UnityEngine;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZGradient : CZType<Gradient>
    {
        public CZGradient() : base()
        { Value = new Gradient(); }

        public CZGradient(Gradient _value) : base(_value) { }

        public static implicit operator CZGradient(Gradient _other) { return new CZGradient(_other); }
    }
}
