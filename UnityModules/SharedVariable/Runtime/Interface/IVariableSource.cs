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
 *  Blog: https://www.mindgear.net/
 *
 */
#endregion
using System.Collections.Generic;

namespace CZToolKit.SharedVariable
{
    public interface IVariableSource
    {
        IVariableOwner VarialbeOwner { get; }

        IReadOnlyList<SharedVariable> Variables { get; }
    }
}