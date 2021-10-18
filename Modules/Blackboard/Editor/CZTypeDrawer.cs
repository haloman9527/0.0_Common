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

namespace CZToolKit.Core.Blackboards.Editors
{
    [CustomPropertyDrawer(typeof(ICZType), true)]
    public class CZTypeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty valueProperty = property.FindPropertyRelative("value");
            if (valueProperty == null)
            {
                EditorGUI.HelpBox(position, $"{label.text}:类型不确定", MessageType.Error);
                return;
            }

#if ODIN_INSPECTOR
            if (valueProperty.isArray && valueProperty.propertyType != SerializedPropertyType.String && string.IsNullOrEmpty(label.text))
                label = new GUIContent(property.displayName + "(Array)", label.image, label.tooltip);
#endif

            EditorGUI.PropertyField(position, valueProperty, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty valueProperty = property.FindPropertyRelative("value");
            if (valueProperty == null)
                return base.GetPropertyHeight(property, label);
            return EditorGUI.GetPropertyHeight(valueProperty, label, true);
        }
    }
}
#endif