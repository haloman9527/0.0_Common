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
    public class CZRect : CZType<Rect>
    {
        public CZRect() : base()
        { Value = new Rect(); }

        public CZRect(Rect _value) : base(_value) { }

        public static implicit operator CZRect(Rect _other) { return new CZRect(_other); }
    }
}
