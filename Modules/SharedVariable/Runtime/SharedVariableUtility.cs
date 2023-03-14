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
using System.Reflection;

namespace CZToolKit.Common.SharedVariable
{
    public static class SharedVariableUtility
    {
        public static IEnumerable<SharedVariable> CollectionObjectSharedVariables(object obj)
        {
            Type sharedType = typeof(SharedVariable);
            foreach (var fieldInfo in Util_Reflection.GetFields(obj.GetType(), BindingFlags.Public | BindingFlags.Instance, true))
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
