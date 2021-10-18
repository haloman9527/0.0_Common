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
    public class CZVector4 : CZType<Vector4>
    {
        public CZVector4() : base()
        { Value = Vector4.zero; }

        public CZVector4(Vector4 _value) : base(_value) { }

        public static implicit operator CZVector4(Vector4 _other) { return new CZVector4(_other); }
    }
}
