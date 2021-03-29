using UnityEngine;
using UnityEditor;

namespace CZToolKit.Core.Attributes.Editors
{
    [CustomPropertyDrawer(typeof(HideLabelAttribute))]
    public class HideLabelDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);
            EditorGUI.PropertyField(position, property, new GUIContent(""));
        }
    }
}
