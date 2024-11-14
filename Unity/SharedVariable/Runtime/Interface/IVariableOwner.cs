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
 *  Blog: https://www.haloman.net/
 *
 */
#endregion
using System.Collections.Generic;

namespace CZToolKit.SharedVariable
{
    public interface IVariableOwner
    {
        IVariableSource VariableSource { get; }
        
        IEnumerable<SharedVariable> Variables();
    }

    public interface IVariableSource
    {
        bool TryGetValue<T>(long id, out T value);
        
        bool SetValue<T>(long id, T value);
    }
}
