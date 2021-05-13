#region ×¢ ÊÍ
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
using System.Collections.Generic;

namespace CZToolKit.Core.SharedVariable
{
    public interface IVariableSource
    {
        IVariableOwner VarialbeOwner { get; }

        IReadOnlyList<SharedVariable> Variables { get; }

        void InitializePropertyMapping(IVariableOwner _variableOwner);
    }
}