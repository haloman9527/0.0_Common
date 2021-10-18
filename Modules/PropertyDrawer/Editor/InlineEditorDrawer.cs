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
using UnityEditor;
using UnityEngine;

namespace CZToolKit.Core.Attributes.Editors
{
    [CustomPropertyDrawer(typeof(InlineEditorAttribute))]
    public class InlineEditorDrawer : PropertyDrawer
    {
        Editor editor;
        bool foldout;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label);
            if (EditorGUI.EndChangeCheck() && property.objectReferenceValue != null)
                editor = Editor.CreateEditor(property.objectReferenceValue);

            foldout = EditorGUI.Foldout(position, foldout, string.Empty, true);
            if (foldout)
            {
                if (property.objectReferenceValue != null)
                {
                    if (editor == null)
                        editor = Editor.CreateEditor(property.objectReferenceValue);
                    editor.OnInspectorGUI();
                }
            }
        }
    }
}
#endif