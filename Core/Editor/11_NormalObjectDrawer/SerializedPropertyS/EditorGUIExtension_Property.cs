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
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace CZToolKit.Core.Editors
{
    public static partial class EditorGUIExtension
    {
        public static float GetPropertyHeight(SerializedPropertyS property)
        {
            return GetPropertyHeight(property, true, property.niceName);
        }

        public static float GetPropertyHeight(SerializedPropertyS property, GUIContent label)
        {
            return GetPropertyHeight(property, true, label);
        }

        public static float GetPropertyHeight(SerializedPropertyS property, bool includeChildren)
        {
            return GetPropertyHeight(property, includeChildren, property.niceName);
        }

        public static float GetPropertyHeight(SerializedPropertyS property, bool includeChildren, GUIContent label)
        {
            if (!property.HasChildren)
                return GetPropertyHeight(property.propertyType, label);

            if (!property.expanded)
                return EditorGUIUtility.singleLineHeight;

            float height = EditorGUIUtility.singleLineHeight;

            if (includeChildren)
            {
                foreach (var children in property.GetIterator())
                {
                    height += GetPropertyHeight(children) + EditorGUIUtility.standardVerticalSpacing;
                }
            }
            return height;
        }

        public static void PropertyField(Rect rect, SerializedPropertyS property)
        {
            if (property.HasChildren)
            {
                rect.height = EditorGUIUtility.singleLineHeight;
                property.expanded = EditorGUI.Foldout(rect, property.expanded, property.niceName);
                EditorGUI.indentLevel++;
                if (property.expanded)
                {
                    foreach (var children in property.GetIterator())
                    {
                        rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                        rect.height = GetPropertyHeight(children);

                        PropertyField(rect, children);
                    }
                }
                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                var value = DrawField(rect, property.propertyType, property.Value, property.niceName);
                if (EditorGUI.EndChangeCheck())
                {
                    property.Value = value;
                }
            }
        }
    }
}
#endif