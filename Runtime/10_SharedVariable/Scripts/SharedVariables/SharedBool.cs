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

namespace CZToolKit.Core.SharedVariable
{
    [Serializable]
    public class SharedBool : SharedVariable<bool>
    {
        public SharedBool() : base() { }

        public SharedBool(bool _value) : base(_value) { }

        public override object Clone()
        {
            SharedBool variable = new SharedBool(Value) { GUID = this.GUID };
            return variable;
        }
    }
}
