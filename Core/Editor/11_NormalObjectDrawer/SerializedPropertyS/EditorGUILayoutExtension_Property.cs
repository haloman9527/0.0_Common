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

namespace CZToolKit.Core.Editors
{
    public static partial class EditorGUILayoutExtension
    {
        public static void PropertyField(SerializedPropertyS property)
        {
            var height = EditorGUIUtility.singleLineHeight;
            if (property.drawer != null)
            {
                height = EditorGUIExtension.GetPropertyHeight(property);
                var rect = EditorGUILayout.GetControlRect(GUILayout.Height(height));
                property.drawer.OnGUI(rect, property.niceName);
            }
            else if (property.HasChildren)
            {
                var rect = EditorGUILayout.GetControlRect(GUILayout.Height(height));
                property.expanded = EditorGUI.Foldout(rect, property.expanded, property.niceName);
                EditorGUI.indentLevel++;
                if (property.expanded)
                {
                    foreach (var children in property.GetIterator())
                    {
                        PropertyField(children);
                    }
                }
                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                height = EditorGUIExtension.GetHeight(property.propertyType, property.niceName);
                var rect = EditorGUILayout.GetControlRect(GUILayout.Height(height));
                var value = EditorGUIExtension.DrawField(rect, property.propertyType, property.Value, property.niceName);
                if (EditorGUI.EndChangeCheck())
                {
                    property.Value = value;
                }
            }
        }
    }
}
#endif