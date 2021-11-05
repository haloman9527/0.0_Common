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
#if UNITY_EDITOR
using CZToolKit.Core.Editors;
using UnityEditor;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace CZToolKit.Core.SharedVariable.Editors
{
    //[CustomFieldDrawer(typeof(SharedVariableAttribute))]
    public class SharedVariableFieldDrawer : ObjectDrawer
    {
        public override void OnGUI(Rect position, GUIContent label)
        {
            SharedVariable variable = Target as SharedVariable;
            IVariableOwner variableOwner = variable.VariableOwner;
            if (variableOwner == null) { EditorGUILayout.HelpBox("没有VariableOwner", MessageType.Error); return; }
            //EditorGUILayout.HelpBox("ReferenceType:" + variable.GUID, MessageType.Info);
            //if (variableOwner.GetVariable(variable.GUID) == null)
            //    variableOwner.SetVariable(variable.Clone() as SharedVariable);
            EditorGUI.BeginChangeCheck();
            object value = EditorGUILayoutExtension.DrawField(variable.GetValue(), label);
            if (EditorGUI.EndChangeCheck())
            {
                variable.SetValue(value);
                EditorUtility.SetDirty(variableOwner as UnityObject);
            }
        }
    }
}
#endif