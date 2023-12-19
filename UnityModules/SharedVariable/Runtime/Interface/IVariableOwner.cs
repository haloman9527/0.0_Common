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
using System.Collections.Generic;

namespace CZToolKit.SharedVariable
{
    public interface IVariableOwner
    {
        SharedVariable GetVariable(string guid);

        void SetVariable(SharedVariable variable);

        void SetVariableValue(string guid, object value);

        IReadOnlyList<SharedVariable> GetVariables();

        void SetVariables(List<SharedVariable> variables);
    }
}
