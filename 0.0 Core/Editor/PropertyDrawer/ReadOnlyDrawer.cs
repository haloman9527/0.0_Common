using UnityEditor;
using UnityEngine;

namespace CZToolKit.Core.Attributes.Editors
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute),true)]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}
