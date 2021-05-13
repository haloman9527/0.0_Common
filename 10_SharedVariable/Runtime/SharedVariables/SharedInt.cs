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
    public class SharedInt : SharedVariable<int>
    {
        public SharedInt() : base() { }

        public SharedInt(int _value) : base(_value) { }

        public override object Clone()
        {
            SharedInt variable = new SharedInt(Value) { GUID = this.GUID };
            return variable;
        }
    }
}
