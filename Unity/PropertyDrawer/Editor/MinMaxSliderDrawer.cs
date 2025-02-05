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
using UnityEditor;
using UnityEngine;

namespace Moyo.UnityEditors
{
    [global::UnityEditor.CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    public class MinMaxSliderDrawer : global::UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            MinMaxSliderAttribute minMaxSliderAttribute = (attribute as MinMaxSliderAttribute);
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();

            float min = minMaxSliderAttribute.min;
            float max = minMaxSliderAttribute.max;
            Vector2 value = property.vector2Value;

            Rect rect = position;
            rect.width = minMaxSliderAttribute.fieldWidth;
            value.x = Mathf.Clamp(EditorGUI.FloatField(rect, value.x), min, value.y);

            rect.x += rect.width + 5;
            rect.width = position.width - minMaxSliderAttribute.fieldWidth * 2 - 10;
            EditorGUI.MinMaxSlider(rect, ref value.x, ref value.y, min, max);

            rect.x += rect.width + 5;
            rect.width = minMaxSliderAttribute.fieldWidth;
            value.y = Mathf.Clamp(EditorGUI.FloatField(rect, value.y), value.x, max);

            property.vector2Value = value;

            GUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }
    }
}
#endif