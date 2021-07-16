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
#region ע ��
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