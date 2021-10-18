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
    public class CZVector2 : CZType<Vector2>
    {
        public CZVector2() : base()
        { Value = Vector2.zero; }

        public CZVector2(Vector2 _value) : base(_value) { }

        public static implicit operator CZVector2(Vector2 _other) { return new CZVector2(_other); }
    }
}
