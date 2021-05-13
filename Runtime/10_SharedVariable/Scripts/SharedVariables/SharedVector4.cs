#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 
 *
 */
#endregion
using System;
using UnityEngine;

namespace CZToolKit.Core.SharedVariable
{
    [Serializable]
    public class SharedVector4 : SharedVariable<Vector4>
    {
        public SharedVector4() : base() { }

        public SharedVector4(Vector4 _value) : base(_value) { }

        public override object Clone()
        {
            SharedVector4 variable = new SharedVector4(Value) { GUID = this.GUID };
            return variable;
        }
    }
}
