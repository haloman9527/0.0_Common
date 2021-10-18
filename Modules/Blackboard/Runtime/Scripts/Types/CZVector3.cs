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
    public class CZVector3 : CZType<Vector3>
    {
        public CZVector3() : base()
        { Value = Vector3.zero; }

        public CZVector3(Vector3 _value) : base(_value) { }

        public static implicit operator CZVector3(Vector3 _other) { return new CZVector3(_other); }
    }
}
