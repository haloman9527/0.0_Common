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
            List<FieldInfo> fieldInfos = Utility.GetFieldInfos(_object.GetType());
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
                //else if (typeof(IList).IsAssignableFrom(fieldInfo.FieldType))
                //{
                //    Type elementType;
                //    if (fieldInfo.FieldType.IsArray)
                //        elementType = fieldInfo.FieldType.GetElementType();
                //    else
                //    {
                //        Type type2 = fieldInfo.FieldType;
                //        while (!type2.IsGenericType)
                //        {
                //            type2 = type2.BaseType;
                //        }
                //        elementType = type2.GetGenericArguments()[0];
                //    }
                //    if (sharedType.IsAssignableFrom(elementType))
                //    {
                //        IList list = fieldInfo.GetValue(_object) as IList;
                //        foreach (var v in list)
                //        {
                //            SharedVariable variable = v as SharedVariable;
                //            if (variable == null)
                //            {
                //                variable = Activator.CreateInstance(fieldInfo.FieldType) as SharedVariable;
                //                fieldInfo.SetValue(_object, variable);
                //            }
                //            yield return variable;
                //        }
                //    }
                //}
            }
        }

    }
}
