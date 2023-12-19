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
 *  Github: https://github.com/haloman9527
 *  Blog: https://www.mindgear.net/
 *
 */
#endregion
using System;

namespace CZToolKit.SharedVariable
{
    [Serializable]
    public class SharedString : SharedVariable<string>
    {
        public SharedString() : base() { }

        public SharedString(string _value) : base(_value) { }

        public override object Clone()
        {
            SharedString variable = new SharedString(Value) { GUID = this.GUID };
            return variable;
        }
    }
}
