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

namespace CZToolKit.Core.SharedVariable
{
    [Serializable]
    public class SharedVector3 : SharedVariable<Vector3>
    {
        public SharedVector3() : base() { }

        public SharedVector3(Vector3 _value) : base(_value) { }

        public override object Clone()
        {
            SharedVector3 variable = new SharedVector3(Value) { GUID = this.GUID };
            return variable;
        }
    }
}
