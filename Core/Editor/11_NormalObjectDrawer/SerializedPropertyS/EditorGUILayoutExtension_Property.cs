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
            EditorGUIExtension.PropertyField(EditorGUILayout.GetControlRect(GUILayout.Height(EditorGUIExtension.GetPropertyHeight(property))), property);
        }
    }
}
#endif