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
using Moyo.Unity;
using UnityEngine;
using UnityEditor;

namespace Moyo.UnityEditors
{
    [global::UnityEditor.CustomPropertyDrawer(typeof(HideLabelAttribute))]
    public class HideLabelDrawer : global::UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);
            EditorGUI.PropertyField(position, property, GUIContent.none);
        }
    }
}
#endif