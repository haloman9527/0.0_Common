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
    public class SharedTransform : SharedObject<Transform>
    {
        public SharedTransform() : base() { }

        public SharedTransform(Transform _value) : base(_value) { }

        public override object Clone()
        {
            SharedTransform variable = new SharedTransform(Value) { GUID = this.GUID };
            return variable;
        }
    }
}
