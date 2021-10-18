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
    public class CZVector3Int : CZType<Vector3Int>
    {
        public CZVector3Int() : base()
        { Value = Vector3Int.zero; }

        public CZVector3Int(Vector3Int _value) : base(_value) { }

        public static implicit operator CZVector3Int(Vector3Int _other) { return new CZVector3Int(_other); }
    }
}
