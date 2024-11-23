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
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Jiange.SharedVariable
{
    public static class SharedVariableUtility
    {
        public static IEnumerable<SharedVariable> CollectionObjectSharedVariables(object obj)
        {
            Type sharedType = typeof(SharedVariable);
            foreach (var fieldInfo in Util_Reflection.GetFields(obj.GetType(), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
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
                }
            }
        }
    }
}
