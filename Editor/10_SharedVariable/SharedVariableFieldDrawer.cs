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
using CZToolKit.Core.Editors;
using UnityEditor;
using UnityEngine;

namespace CZToolKit.Core.SharedVariable.Editors
{
    [CustomFieldDrawer(typeof(SharedVariableAttribute))]
    public class SharedVariableFieldDrawer : FieldDrawer
    {
        public override void OnGUI(GUIContent label)
        {
            SharedVariable variable = Value as SharedVariable;
            IVariableOwner variableOwner = variable.VariableOwner;
            if (variableOwner == null) { EditorGUILayout.HelpBox("没有VariableOwner", MessageType.Error); return; }
            EditorGUILayout.HelpBox("ReferenceType:" + variable.GUID, MessageType.Info);
            variable.SetValue(EditorGUILayoutExtension.DrawField(label, variable.GetValueType(), variable.GetValue()));
            if (GUI.changed)
                EditorUtility.SetDirty(variableOwner.GetObject());
        }
    }
}
