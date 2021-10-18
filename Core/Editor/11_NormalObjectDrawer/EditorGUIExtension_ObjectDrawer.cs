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
using UnityEditor;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace CZToolKit.Core.Editors
{
    public static partial class EditorGUIExtension
    {
        public static float GetPropertyHeight(Type _type, GUIContent label)
        {
            if (_type.Equals(typeof(bool)))
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Boolean, label);
            if (_type.Equals(typeof(byte)) || _type.Equals(typeof(sbyte))
                || _type.Equals(typeof(short)) || _type.Equals(typeof(ushort))
                || _type.Equals(typeof(int)) || _type.Equals(typeof(uint))
                || _type.Equals(typeof(long)) || _type.Equals(typeof(ulong)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Integer, label);
            }
            if (_type.Equals(typeof(float)) || _type.Equals(typeof(double)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Float, label);
            }
            if (_type.Equals(typeof(char)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Character, label);
            }
            if (_type.Equals(typeof(string)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.String, label);
            }
            if (_type.Equals(typeof(Color)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Color, label);
            }
            if (_type.Equals(typeof(LayerMask)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.LayerMask, label);
            }
            if (_type.IsEnum)
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Enum, label);
            }
            if (typeof(UnityEngine.Object).IsAssignableFrom(_type))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.ObjectReference, label);
            }
            if (_type.Equals(typeof(Vector2)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector2, label);
            }
            if (_type.Equals(typeof(Vector2Int)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector2Int, label);
            }
            if (_type.Equals(typeof(Vector3)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector3, label);
            }
            if (_type.Equals(typeof(Vector3Int)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector3Int, label);
            }
            if (_type.Equals(typeof(Vector4)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector4, label);
            }
            if (_type.Equals(typeof(Quaternion)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Quaternion, label);
            }
            if (_type.Equals(typeof(Rect)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Rect, label);
            }
            if (_type.Equals(typeof(RectInt)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.RectInt, label);
            }
            if (_type.Equals(typeof(Gradient)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Gradient, label);
            }
            if (_type.Equals(typeof(AnimationCurve)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.AnimationCurve, label);
            }
            if (_type.Equals(typeof(Bounds)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Bounds, label);
            }
            if (_type.Equals(typeof(BoundsInt)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.BoundsInt, label);
            }
            return EditorGUIUtility.singleLineHeight;
        }


        public static object DrawField(Rect _rect, GUIContent _content, Type _fieldType, object _value)
        {
            if (_fieldType.Equals(typeof(int)))
            {
                return EditorGUI.IntField(_rect, _content, _value == null ? 0 : (int)_value, EditorStylesExtension.NumberFieldStyle);
            }
            if (_fieldType.Equals(typeof(float)))
            {
                return EditorGUI.FloatField(_rect, _content, _value == null ? 0 : (float)_value, EditorStylesExtension.NumberFieldStyle);
            }
            if (_fieldType.Equals(typeof(double)))
            {
                return EditorGUI.FloatField(_rect, _content, Convert.ToSingle(_value == null ? 0 : (double)_value), EditorStylesExtension.NumberFieldStyle);
            }
            if (_fieldType.Equals(typeof(long)))
            {
                return (long)EditorGUI.IntField(_rect, _content, Convert.ToInt32(_value == null ? 0 : (long)_value), EditorStylesExtension.NumberFieldStyle);
            }
            if (_fieldType.Equals(typeof(bool)))
            {
                return EditorGUI.Toggle(_rect, _content, _value == null ? false : (bool)_value);
            }
            if (_fieldType.Equals(typeof(string)))
            {
                return EditorGUI.TextField(_rect, _content, _value == null ? "" : (string)_value, EditorStylesExtension.TextFieldStyle);
            }
            if (_fieldType.Equals(typeof(byte)))
            {
                return Convert.ToByte(EditorGUI.IntField(_rect, _content, Convert.ToInt32(_value == null ? 0 : (byte)_value)));
            }
            if (_fieldType.Equals(typeof(Vector2)))
            {
                return EditorGUI.Vector2Field(_rect, _content, _value == null ? Vector2.zero : (Vector2)_value);
            }
            if (_fieldType.Equals(typeof(Vector2Int)))
            {
                return EditorGUI.Vector2IntField(_rect, _content, _value == null ? Vector2Int.zero : (Vector2Int)_value);
            }
            if (_fieldType.Equals(typeof(Vector3)))
            {
                return EditorGUI.Vector3Field(_rect, _content, _value == null ? Vector3.zero : (Vector3)_value);
            }
            if (_fieldType.Equals(typeof(Vector3Int)))
            {
                return EditorGUI.Vector3IntField(_rect, _content, _value == null ? Vector3Int.zero : (Vector3Int)_value);
            }
            if (_fieldType.Equals(typeof(Vector4)))
            {
                return EditorGUI.Vector4Field(_rect, _content, _value == null ? Vector4.zero : (Vector4)_value);
            }
            if (_fieldType.Equals(typeof(Quaternion)))
            {
                Quaternion quaternion = _value == null ? Quaternion.identity : (Quaternion)_value;
                Vector4 vector = Vector4.zero;
                vector.Set(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
                vector = EditorGUI.Vector4Field(_rect, _content, vector);
                quaternion.Set(vector.x, vector.y, vector.z, vector.w);
                return quaternion;
            }
            if (_fieldType.Equals(typeof(Color)))
            {
                return EditorGUI.ColorField(_rect, _content, _value == null ? Color.black : (Color)_value);
            }
            if (_fieldType.Equals(typeof(Rect)))
            {
                return EditorGUI.RectField(_rect, _content, _value == null ? Rect.zero : (Rect)_value);
            }
            if (_fieldType.Equals(typeof(AnimationCurve)))
            {
                if (_value == null)
                {
                    _value = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
                    GUI.changed = true;
                }
                return EditorGUI.CurveField(_rect, _content, (AnimationCurve)_value);
            }
            if (_fieldType.Equals(typeof(LayerMask)))
            {
                LayerMask l = (LayerMask)(_value == null ? (LayerMask)(-1) : _value);
                return (LayerMask)EditorGUI.LayerField(_rect, _content, l.value);
            }
            if (typeof(UnityObject).IsAssignableFrom(_fieldType))
            {
                return EditorGUI.ObjectField(_rect, _content, (UnityObject)_value, _fieldType, true);
            }
            if (_fieldType.IsEnum)
            {
                return EditorGUI.EnumPopup(_rect, _content, (Enum)_value);
            }
            EditorGUILayout.LabelField("Unsupported Type: " + _fieldType);
            return null;
        }
    }
}
#endif