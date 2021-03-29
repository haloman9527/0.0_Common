using CZToolKit.Core.Editors;
using UnityEditor;
using UnityEngine;

namespace CZToolKit.Core.Attributes.Editors
{
    [CustomPropertyDrawer(typeof(ProgressBarAttribute))]
    public class ProgressBarDrawer : PropertyDrawer
    {
        ProgressBarAttribute progressBarAttribute;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            progressBarAttribute = (attribute as ProgressBarAttribute);
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    property.intValue = (int)EditorGUIExtension.ProgressBar(position, property.intValue,
                        progressBarAttribute.min,
                        progressBarAttribute.max,
                        string.Concat(property.intValue.ToString(), "(", property.displayName, ")")
                        , true, progressBarAttribute.drawMinMaxValue);
                    break;
                case SerializedPropertyType.Float:
                    property.floatValue = EditorGUIExtension.ProgressBar(position, property.floatValue,
                        progressBarAttribute.min,
                        progressBarAttribute.max,
                        string.Concat(property.floatValue.ToString("0.00"),"(", "property.displayName", ")"),
                        true, progressBarAttribute.drawMinMaxValue);
                    break;
                default:
                    break;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            progressBarAttribute = (attribute as ProgressBarAttribute);
            return progressBarAttribute.height;
        }
    }
}
