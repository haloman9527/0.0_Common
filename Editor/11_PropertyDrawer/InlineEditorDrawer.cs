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
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using CZToolKit.Core.Editors;

namespace CZToolKit.Core.Attributes.Editors
{
    [CustomPropertyDrawer(typeof(InlineEditorAttribute))]
    public class InlineEditorDrawer : PropertyDrawer
    {
        Editor editor;
        bool foldout;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (foldout = EditorGUILayout.Foldout(foldout, label, true))
            {
                if (property.objectReferenceValue != null)
                {
                    if (editor == null)
                        editor = Editor.CreateEditor(property.objectReferenceValue);
                    editor.OnInspectorGUI();
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
            //return base.GetPropertyHeight(property, label);
        }
    }
}
