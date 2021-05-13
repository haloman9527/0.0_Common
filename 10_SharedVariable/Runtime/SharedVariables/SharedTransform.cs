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
    public class SharedTransform : SharedVariable<Transform>
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
