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
 *  Blog: https://www.mindgear.net/
 *
 */
#endregion
#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace CZToolKit.Attributes.Editors
{
    [CustomPropertyDrawer(typeof(PreviewObjectFieldAttribute), true)]
    public class PreviewObjectFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.objectReferenceValue = EditorGUILayout.ObjectField(label, property.objectReferenceValue as Texture2D, typeof(Texture2D), false);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, true);
        }
    }
}
#endif