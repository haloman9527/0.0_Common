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
using System.Collections.Generic;

namespace CZToolKit.Core.SharedVariable
{
    public static class SharedVariableUtility
    {
        public static IEnumerable<SharedVariable> CollectionObjectSharedVariables(object obj)
        {
            Type sharedType = typeof(SharedVariable);
            foreach (var fieldInfo in Util_Reflection.GetFieldInfos(obj.GetType()))
            {
                if (sharedType.IsAssignableFrom(fieldInfo.FieldType))
                {
                    SharedVariable variable = fieldInfo.GetValue(obj) as SharedVariable;
                    if (variable == null)
                    {
                        variable = Activator.CreateInstance(fieldInfo.FieldType) as SharedVariable;
                        fieldInfo.SetValue(obj, variable);
                    }
                    yield return variable;
                    continue;
                }
            }
        }
    }
}
