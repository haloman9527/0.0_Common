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
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace CZToolKit.Core.Editors
{
    public static partial class EditorGUIExtension
    {
        static Dictionary<Type, float> heightMap;
        static Dictionary<Type, float> HeightMap
        {
            get
            {
                if (heightMap == null)
                {
                    heightMap = new Dictionary<Type, float>();

                }
                return heightMap;
            }
        }

        public static object CreateInstance(Type type)
        {
            if (type == typeof(string))
                return "";
            if (type.IsArray)
                return Array.CreateInstance(type.GetElementType(), 0);
            return Activator.CreateInstance(type, true);
        }

        /// <summary> 是否是基元类型 </summary>
        public static bool IsBasicType(Type type)
        {
            if (type.Equals(typeof(bool))) return true;
            if (type.Equals(typeof(byte))) return true;
            if (type.Equals(typeof(sbyte))) return true;
            if (type.Equals(typeof(short))) return true;
            if (type.Equals(typeof(ushort))) return true;
            if (type.Equals(typeof(int))) return true;
            if (type.Equals(typeof(uint))) return true;
            if (type.Equals(typeof(float))) return true;
            if (type.Equals(typeof(double))) return true;
            if (type.Equals(typeof(long))) return true;
            if (type.Equals(typeof(ulong))) return true;
            if (type.Equals(typeof(string))) return true;
            if (type.Equals(typeof(char))) return true;
            if (type.Equals(typeof(Vector2))) return true;
            if (type.Equals(typeof(Vector2Int))) return true;
            if (type.Equals(typeof(Vector3))) return true;
            if (type.Equals(typeof(Vector3Int))) return true;
            if (type.Equals(typeof(Vector4))) return true;
            if (type.Equals(typeof(Quaternion))) return true;
            if (type.Equals(typeof(Color))) return true;
            if (type.Equals(typeof(Rect))) return true;
            if (type.Equals(typeof(RectInt))) return true;
            if (type.Equals(typeof(Bounds))) return true;
            if (type.Equals(typeof(BoundsInt))) return true;
            if (type.Equals(typeof(LayerMask))) return true;
            if (type.IsEnum) return true;
            if (typeof(Gradient).IsAssignableFrom(type)) return true;
            if (typeof(AnimationCurve).IsAssignableFrom(type)) return true;
            if (typeof(UnityObject).IsAssignableFrom(type)) return true;
            return false;
        }

        /// <returns> 是否支持绘制此类型数据 </returns>
        public static bool IsSupport(Type type)
        {
            if (IsBasicType(type))
            {
                return true;
            }

            //if (typeof(IList).IsAssignableFrom(type))
            //{
            //    if (type.IsArray)
            //        type = type.GetElementType();
            //    else
            //    {
            //        Type type2 = type;
            //        while (!type2.IsGenericType)
            //        {
            //            type2 = type2.BaseType;
            //        }
            //        type = type2.GetGenericArguments()[0];
            //    }
            //}

            //if (type.IsClass || (type.IsValueType && !type.IsPrimitive))
            //{
            //    if (!typeof(Delegate).IsAssignableFrom(type) && typeof(object).IsAssignableFrom(type))
            //        return true;
            //}
            return false;
        }

        public static float GetHeight(Type type, GUIContent label)
        {
            if (type.Equals(typeof(bool)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Boolean, label);
            }
            if (type.Equals(typeof(byte)) || type.Equals(typeof(sbyte))
                || type.Equals(typeof(short)) || type.Equals(typeof(ushort))
                || type.Equals(typeof(int)) || type.Equals(typeof(uint))
                || type.Equals(typeof(long)) || type.Equals(typeof(ulong)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Integer, label);
            }
            if (type.Equals(typeof(float)) || type.Equals(typeof(double)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Float, label);
            }
            if (type.Equals(typeof(char)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Character, label);
            }
            if (type.Equals(typeof(string)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.String, label);
            }
            if (type.Equals(typeof(Color)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Color, label);
            }
            if (type.Equals(typeof(LayerMask)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.LayerMask, label);
            }
            if (type.Equals(typeof(Vector2)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector2, label);
            }
            if (type.Equals(typeof(Vector2Int)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector2Int, label);
            }
            if (type.Equals(typeof(Vector3)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector3, label);
            }
            if (type.Equals(typeof(Vector3Int)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector3Int, label);
            }
            if (type.Equals(typeof(Vector4)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector4, label);
            }
            if (type.Equals(typeof(Quaternion)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Quaternion, label);
            }
            if (type.Equals(typeof(Rect)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Rect, label);
            }
            if (type.Equals(typeof(RectInt)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.RectInt, label);
            }
            if (type.Equals(typeof(Bounds)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Bounds, label);
            }
            if (type.Equals(typeof(BoundsInt)))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.BoundsInt, label);
            }
            if (type.IsEnum)
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Enum, label);
            }
            if (typeof(Gradient).IsAssignableFrom(type))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.Gradient, label);
            }
            if (typeof(AnimationCurve).IsAssignableFrom(type))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.AnimationCurve, label);
            }
            if (typeof(UnityObject).IsAssignableFrom(type))
            {
                return EditorGUI.GetPropertyHeight(SerializedPropertyType.ObjectReference, label);
            }

            return 0;
        }

        public static object DrawField(Rect rect, Type type, object value, GUIContent label)
        {
            if (value == null)
            {
                if (!typeof(UnityObject).IsAssignableFrom(type))
                    value = CreateInstance(type);
            }
            if (!IsSupport(type))
            {
                return null;
            }
            if (type.Equals(typeof(bool)))
            {
                return EditorGUI.Toggle(rect, label, value == null ? false : (bool)value);
            }
            if (type.Equals(typeof(short)) || type.Equals(typeof(ushort))
                || type.Equals(typeof(int)) || type.Equals(typeof(uint)))
            {
                return EditorGUI.IntField(rect, label, value == null ? 0 : (int)value);
            }
            if (type.Equals(typeof(long)) || type.Equals(typeof(ulong)))
            {
                return EditorGUI.LongField(rect, label, value == null ? 0 : (long)value);
            }
            if (type.Equals(typeof(float)))
            {
                return EditorGUI.FloatField(rect, label, value == null ? 0 : (float)value);
            }
            if (type.Equals(typeof(double)))
            {
                return EditorGUI.DoubleField(rect, label, value == null ? 0 : (double)value);
            }
            if (type.Equals(typeof(string)))
            {
                return EditorGUI.TextField(rect, label, value == null ? "" : (string)value);
            }
            if (type.Equals(typeof(Vector2)))
            {
                return EditorGUI.Vector2Field(rect, label, value == null ? Vector2.zero : (Vector2)value);
            }
            if (type.Equals(typeof(Vector2Int)))
            {
                return EditorGUI.Vector2IntField(rect, label, value == null ? Vector2Int.zero : (Vector2Int)value);
            }
            if (type.Equals(typeof(Vector3)))
            {
                return EditorGUI.Vector3Field(rect, label, value == null ? Vector3.zero : (Vector3)value);
            }
            if (type.Equals(typeof(Vector3Int)))
            {
                return EditorGUI.Vector3IntField(rect, label, value == null ? Vector3Int.zero : (Vector3Int)value);
            }
            if (type.Equals(typeof(Vector4)))
            {
                return EditorGUI.Vector4Field(rect, label, value == null ? Vector4.zero : (Vector4)value);
            }
            if (type.Equals(typeof(Quaternion)))
            {
                Quaternion quaternion = value == null ? Quaternion.identity : (Quaternion)value;
                Vector4 vector = new Vector4(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
                vector = EditorGUI.Vector4Field(rect, label, vector);
                quaternion.Set(vector.x, vector.y, vector.z, vector.w);
                return quaternion;
            }
            if (type.Equals(typeof(Color)))
            {
                return EditorGUI.ColorField(rect, label, value == null ? Color.black : (Color)value);
            }
            if (type.Equals(typeof(Rect)))
            {
                return EditorGUI.RectField(rect, label, value == null ? Rect.zero : (Rect)value);
            }
            if (type.Equals(typeof(AnimationCurve)))
            {
                return EditorGUI.CurveField(rect, label, value == null ? AnimationCurve.EaseInOut(0f, 0f, 1f, 1f) : (AnimationCurve)value);
            }
            if (type.Equals(typeof(LayerMask)))
            {
                return (LayerMask)EditorGUI.LayerField(rect, label, (LayerMask)(value == null ? (-1) : value));
            }
            if (typeof(UnityObject).IsAssignableFrom(type))
            {
                return EditorGUI.ObjectField(rect, label, (UnityObject)value, type, true);
            }
            if (type.IsEnum)
            {
                return EditorGUI.EnumPopup(rect, label, (Enum)value);
            }
            return null;
        }

        public static object DrawField(Rect rect, Type type, object value, string label)
        {
            return DrawField(rect, type, value, GUIHelper.TextContent(label));
        }

        public static object DrawField(Rect rect, object value, GUIContent label)
        {
            return DrawField(rect, value.GetType(), value, label);
        }

        public static object DrawField(Rect rect, object value, string label)
        {
            return DrawField(rect, value.GetType(), value, label);
        }
    }
}
#endif