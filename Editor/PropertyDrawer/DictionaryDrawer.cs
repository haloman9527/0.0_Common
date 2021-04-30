using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace CZToolKit.Core.Attributes.Editors
{
    [CustomPropertyDrawer(typeof(DictionaryAttribute))]
    public class DictionaryDrawer : PropertyDrawer
    {
        ReorderableList reorderableList;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return null;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);
            if (reorderableList == null)
            {
                SerializedProperty keysProperty = property.FindPropertyRelative("keys");
                SerializedProperty valuesProperty = property.FindPropertyRelative("values");
                reorderableList = new ReorderableList(property.serializedObject, keysProperty, false, false, true, true);

                reorderableList.elementHeightCallback = (_index) =>
                {
                    return EditorGUI.GetPropertyHeight(valuesProperty.GetArrayElementAtIndex(_index));
                };
                reorderableList.drawElementCallback = (_rect, _index, _isActive, _isFocused) =>
                {
                    SerializedProperty key = keysProperty.GetArrayElementAtIndex(_index);
                    SerializedProperty value = valuesProperty.GetArrayElementAtIndex(_index);
                    Rect keyRect = _rect;
                    keyRect.width = 72;

                    Rect valueRect = _rect;
                    valueRect.x += keyRect.width;
                    valueRect.width = _rect.width - keyRect.width;
                    EditorGUI.PropertyField(keyRect, key, new GUIContent(""));
                    EditorGUI.PropertyField(valueRect, value, new GUIContent(""), true);
                };

                reorderableList.onAddCallback = (list) =>
                {
                    keysProperty.InsertArrayElementAtIndex(0);
                    valuesProperty.InsertArrayElementAtIndex(0);
                };
            }
            Rect foldoutRect = position;
            //foldoutRect.y -= foldoutRect.height / 2 - 10;
            foldoutRect.height = 20;

            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);
            if (property.isExpanded)
            {
                position.y += foldoutRect.height;
                reorderableList.DoList(position);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty valuesProperty = property.FindPropertyRelative("values");
            if (!property.isExpanded)
            {

                return base.GetPropertyHeight(property, label);
            }
            return EditorGUI.GetPropertyHeight(valuesProperty, true) + 20;
        }
    }
}
