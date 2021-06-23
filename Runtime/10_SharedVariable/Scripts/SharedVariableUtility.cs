#region 注 释
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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CZToolKit.Core.SharedVariable
{
    public static class SharedVariableUtility
    {
        public static IEnumerable<SharedVariable> CollectionObjectSharedVariables(object _object)
        {
            List<FieldInfo> fieldInfos = Utility_Reflection.GetFieldInfos(_object.GetType());
            Type sharedType = typeof(SharedVariable);
            foreach (var fieldInfo in fieldInfos)
            {
                if (sharedType.IsAssignableFrom(fieldInfo.FieldType))
                {
                    SharedVariable variable = fieldInfo.GetValue(_object) as SharedVariable;
                    if (variable == null)
                    {
                        variable = Activator.CreateInstance(fieldInfo.FieldType) as SharedVariable;
                        fieldInfo.SetValue(_object, variable);
                    }
                    yield return variable;
                    continue;
                }
            }
        }

    }
}
