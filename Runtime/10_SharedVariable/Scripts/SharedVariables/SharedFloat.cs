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

namespace CZToolKit.Core.SharedVariable
{
    [Serializable]
    public class SharedFloat : SharedVariable<float>
    {
        public SharedFloat() : base() { }

        public SharedFloat(float _value) : base(_value) { }

        public override object Clone()
        {
            SharedFloat variable = new SharedFloat(Value) { GUID = this.GUID };
            return variable;
        }
    }
}
