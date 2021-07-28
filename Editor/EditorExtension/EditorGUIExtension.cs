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
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace CZToolKit.Core.Editors
{
    public static partial class EditorGUIExtension
    {
        static Dictionary<string, GUIContent> GUIContentsCache = new Dictionary<string, GUIContent>();
        public static GUIContent GetGUIContent(string _name)
        {
            GUIContent content;
            if (!GUIContentsCache.TryGetValue(_name, out content))
                content = new GUIContent(_name);
            content.tooltip = string.Empty;
            content.image = null;
            return content;
        }
        public static GUIContent GetGUIContent(string _name, Texture2D _image)
        {
            GUIContent content = GetGUIContent(_name);
            content.image = _image;
            return content;
        }

        public static GUIContent GetGUIContent(string _name, string _tooltip)
        {
            GUIContent content = GetGUIContent(_name);
            content.tooltip = _tooltip;
            return content;
        }

        public static GUIContent GetGUIContent(string _name, string _tooltip, Texture2D _image)
        {
            GUIContent content = GetGUIContent(_name);
            content.tooltip = _tooltip;
            content.image = _image;
            return content;
        }

        static Dictionary<string, bool> FoldoutCache = new Dictionary<string, bool>();
        public static bool GetFoldoutBool(string _key, bool _fallback = false)
        {
            bool result;
            if (!FoldoutCache.TryGetValue(_key, out result))
                result = _fallback;
            return result;
        }

        public static void SetFoldoutBool(string _key, bool _value)
        {
            FoldoutCache[_key] = _value;
        }

        /// <summary> 绘制一个ProgressBar </summary>
        public static float ProgressBar(Rect _rect, float _value, float _minLimit, float _maxLimit, string _text, bool _dragable = true, bool _drawMinMax = false)
        {
            float progress = (_value - _minLimit) / (_maxLimit - _minLimit);

            Rect r = _rect;
            GUI.Box(r, string.Empty);
            r.width = _rect.width * progress;
            EditorGUI.DrawRect(r, new Color(0.07f, 0.56f, 0.9f, 1));

            if (_drawMinMax)
            {
                EditorGUI.LabelField(_rect, _minLimit.ToString());
                EditorGUI.LabelField(_rect, _maxLimit.ToString(), EditorStylesExtension.RightLabelStyle);
            }
            EditorGUI.LabelField(_rect, _text, EditorStylesExtension.MiddleLabelStyle);

            if (_dragable)
#if UNITY_2019_1_OR_NEWER
                return GUI.HorizontalSlider(_rect, _value, _minLimit, _maxLimit, EditorStylesExtension.Transparent, EditorStylesExtension.Transparent, EditorStylesExtension.Transparent);
#else
                return GUI.HorizontalSlider(_rect, _value, _minLimit, _maxLimit, EditorStylesExtension.Transparent, EditorStylesExtension.Transparent);
#endif
            return _value;
        }


        public static object DrawSingleField(Rect _rect, GUIContent _content, Type _fieldType, object _value)
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
