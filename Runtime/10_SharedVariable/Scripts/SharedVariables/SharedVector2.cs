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
    public class SharedVector2 : SharedVariable<Vector2>
    {
        public SharedVector2() : base() { }

        public SharedVector2(Vector2 _value) : base(_value) { }

        public override object Clone()
        {
            SharedVector2 variable = new SharedVector2(Value) { GUID = this.GUID };
            return variable;
        }
    }
}
