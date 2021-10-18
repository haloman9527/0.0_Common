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
using System.Collections.Generic;

namespace CZToolKit.Core.SharedVariable
{
    public interface IVariableOwner
    {
        SharedVariable GetVariable(string _guid);

        void SetVariable(SharedVariable _variable);

        void SetVariableValue(string _guid, object _value);

        IReadOnlyList<SharedVariable> GetVariables();

        void SetVariables(List<SharedVariable> _variables);
    }
}
