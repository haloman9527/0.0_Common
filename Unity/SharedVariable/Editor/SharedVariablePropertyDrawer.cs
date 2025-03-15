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
 *  Blog: https://www.haloman.net/
 *
 */
#endregion
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Atom.SharedVariable.Editors
{
    [CustomPropertyDrawer(typeof(SharedVariable), true)]
    public class SharedVariablePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);
            EditorGUI.PropertyField(position, property.FindPropertyRelative("value"), label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("value"), true);
        }
    }
}
#endif