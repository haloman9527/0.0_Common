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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace CZToolKit.Core.Editors
{
    public static partial class EditorGUILayoutExtension
    {
        public static void DrawFieldsInInspector(object _targetObject, UnityObject _unityOwner = null)
        {
            if (_targetObject is UnityObject)
            {
                Selection.activeObject = _targetObject as UnityObject;
            }
            else
            {
                Selection.activeObject = ObjectInspector.Instance;
                ObjectInspector.Instance.TargetObject = _targetObject;
            }
        }

        public static void DrawFieldsInInspector(string _title, object _targetObject, UnityObject _unityOwner = null)
        {
            DrawFieldsInInspector(_targetObject);
            ObjectInspector.Instance.name = _title;
        }

        public static bool DrawFoldout(int hash, GUIContent guiContent)
        {
            string text = string.Concat(new object[]
            {
                c_EditorPrefsFoldoutKey,
                hash,
                ".",
                guiContent.text
            });
            bool @bool = GUIHelper.GetFoldoutBool(text);
            bool flag = EditorGUILayout.Foldout(@bool, guiContent, true, EditorStylesExtension.FoldoutStyle);
            if (flag != @bool)
                GUIHelper.SetFoldoutBool(text, flag);
            return flag;
        }

        public static void SetFoldout(int hash, bool value, GUIContent guiContent)
        {
            string text = string.Concat(new object[]
            {
                c_EditorPrefsFoldoutKey,
                hash,
                ".",
                guiContent.text
            });
            EditorPrefs.SetBool(text, value);
        }

        /// <summary> 不是<see cref="private"/>、或者标记了<see cref="SerializeField"/>特性，并且没有标记<see cref="NonSerializedAttribute"/>特性，并且没有标记<see cref="HideInInspector"/>特性。 </summary>
        /// <returns> 满足以上条件返回<see cref="true"/> </returns>
        public static bool CanDraw(FieldInfo _fieldInfo)
        {
            return ((!_fieldInfo.IsPrivate && !_fieldInfo.IsFamily) || Utility_Attribute.TryGetFieldInfoAttribute(_fieldInfo, out SerializeField serAtt))
                    && !Utility_Attribute.TryGetTypeAttribute(_fieldInfo.DeclaringType, out NonSerializedAttribute nonAtt)
                    && !Utility_Attribute.TryGetFieldInfoAttribute(_fieldInfo, out HideInInspector hideAtt);
        }

        /// <summary> 绘制内部所有字段 </summary>
        public static object DrawFields(object _object)
        {
            if (_object == null) return null;

            List<FieldInfo> fields = Utility_Reflection.GetFieldInfos(_object.GetType());
            for (int j = 0; j < fields.Count; j++)
            {
                if (CanDraw(fields[j]))
                {
                    EditorGUI.BeginChangeCheck();
                    object value = EditorGUILayoutExtension.DrawField(fields[j], fields[j].GetValue(_object));
                    if (EditorGUI.EndChangeCheck())
                    {
                        fields[j].SetValue(_object, value);
                        GUI.changed = true;
                    }
                }
            }
            return _object;
        }

        public static object DrawField(GUIContent _content, FieldInfo _fieldInfo, object _value)
        {
            if (typeof(IList).IsAssignableFrom(_fieldInfo.FieldType))
                return EditorGUILayoutExtension.DrawArrayField(_content, _fieldInfo, _fieldInfo.FieldType, _value);
            return EditorGUILayoutExtension.DrawSingleField(_content, _fieldInfo, _fieldInfo.FieldType, _value);
        }

        public static object DrawField(string _name, FieldInfo _fieldInfo, object _value)
        {
            return DrawField(GUIHelper.GetGUIContent(_name), _fieldInfo, _value);
        }

        public static object DrawField(FieldInfo _fieldInfo, object _value)
        {
            GUIContent content = null;
            if (Utility_Attribute.TryGetFieldInfoAttribute(_fieldInfo, out TooltipAttribute tooltipAtt))
                content = GUIHelper.GetGUIContent(ObjectNames.NicifyVariableName(_fieldInfo.Name), tooltipAtt.tooltip);
            else
                content = GUIHelper.GetGUIContent(ObjectNames.NicifyVariableName(_fieldInfo.Name));
            return DrawField(content, _fieldInfo, _value);
        }

        public static object DrawField(GUIContent _content, Type _fieldType, object _value)
        {
            if (typeof(IList).IsAssignableFrom(_fieldType))
                return EditorGUILayoutExtension.DrawArrayField(_content, null, _fieldType, _value);
            return EditorGUILayoutExtension.DrawSingleField(_content, null, _fieldType, _value);
        }

        public static object DrawField(string _name, Type _fieldType, object _value)
        {
            if (typeof(IList).IsAssignableFrom(_fieldType))
                return EditorGUILayoutExtension.DrawArrayField(GUIHelper.GetGUIContent(_name), null, _fieldType, _value);
            return EditorGUILayoutExtension.DrawSingleField(GUIHelper.GetGUIContent(_name), null, _fieldType, _value);
        }

        private static object DrawArrayField(GUIContent _content, FieldInfo _fieldInfo, Type _fieldType, object _value)
        {
            Type elementType;
            if (_fieldType.IsArray)
                elementType = _fieldType.GetElementType();
            else
            {
                Type type2 = _fieldType;
                while (!type2.IsGenericType)
                {
                    type2 = type2.BaseType;
                }
                elementType = type2.GetGenericArguments()[0];
            }
            IList list;
            if (_value == null)
            {
                if (_fieldType.IsGenericType || _fieldType.IsArray)
                {
                    _value = list = (Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[]
                    {
                        elementType
                    }), true) as IList);
                }
                else
                {
                    _value = list = (Activator.CreateInstance(_fieldType, true) as IList);
                }
                if (_fieldType.IsArray)
                {
                    Array array = Array.CreateInstance(elementType, list.Count);
                    list.CopyTo(array, 0);
                    _value = list = array;
                }
                GUI.changed = true;
            }
            else
            {
                list = (IList)_value;
            }
            if (EditorGUILayoutExtension.DrawFoldout(_value.GetHashCode(), _content))
            {
                EditorGUI.indentLevel++;
                bool flag = _value.GetHashCode() == EditorGUILayoutExtension.editingFieldHash;
                int count = (!flag) ? list.Count : EditorGUILayoutExtension.savedArraySize;
                int newCount = EditorGUILayout.IntField(GUIHelper.GetGUIContent("Size"), count);
                if (flag && EditorGUILayoutExtension.editingArray && (GUIUtility.keyboardControl != EditorGUILayoutExtension.currentKeyboardControl
                    || (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return)))
                {
                    if (newCount != list.Count)
                    {
                        int currentCount = list.Count;
                        if (newCount > currentCount)
                        {
                            if (_fieldType.IsArray)
                            {
                                Array array2 = Array.CreateInstance(elementType, newCount);
                                int num3 = -1;
                                for (int i = 0; i < newCount; i++)
                                {
                                    if (i < list.Count)
                                    {
                                        num3 = i;
                                    }
                                    if (num3 == -1)
                                    {
                                        break;
                                    }

                                }
                            }
                            else
                            {
                                Type type = list.Count > 0 ? list[list.Count - 1].GetType() : elementType;
                                if (!typeof(UnityObject).IsAssignableFrom(type))
                                {
                                    for (int i = currentCount; i < newCount; i++)
                                        list.Add(Activator.CreateInstance(type, true));
                                }
                                else
                                {
                                    for (int i = currentCount; i < newCount; i++)
                                        list.Add(null);
                                }
                            }
                        }
                        else
                        {
                            if (!_fieldType.IsArray)
                            {
                                while (list.Count > newCount)
                                {
                                    list.RemoveAt(list.Count - 1);
                                }
                            }
                        }
                    }
                    EditorGUILayoutExtension.editingArray = false;
                    EditorGUILayoutExtension.savedArraySize = -1;
                    EditorGUILayoutExtension.editingFieldHash = -1;
                    GUI.changed = true;
                }
                else if (newCount != count)
                {
                    if (!EditorGUILayoutExtension.editingArray)
                    {
                        EditorGUILayoutExtension.currentKeyboardControl = GUIUtility.keyboardControl;
                        EditorGUILayoutExtension.editingArray = true;
                        EditorGUILayoutExtension.editingFieldHash = _value.GetHashCode();
                    }
                    EditorGUILayoutExtension.savedArraySize = newCount;
                }

                if (_fieldInfo != null
                    && (Utility_Attribute.TryGetFieldInfoAttribute(_fieldInfo, out FieldAttribute att)
                    || Utility_Attribute.TryGetTypeAttribute(elementType, out att)))
                {
                    for (int k = 0; k < list.Count; k++)
                    {
                        FieldDrawer objectDrawer;
                        if ((objectDrawer = ObjectDrawerUtility.GetObjectDrawer(att)) != null)
                        {
                            _content.text = "Element " + k;
                            objectDrawer.Value = list[k];
                            objectDrawer.OnGUI(_content);
                            if (objectDrawer.Value != list[k])
                            {
                                list[k] = objectDrawer.Value;
                                GUI.changed = true;
                            }
                        }
                    }
                }
                else
                {
                    for (int k = 0; k < list.Count; k++)
                    {
                        _content.text = "Element " + k;
                        list[k] = EditorGUILayoutExtension.DrawField(_content, elementType, list[k]);
                    }
                }
                EditorGUI.indentLevel--;
            }
            return list;
        }

        private static object DrawSingleField(GUIContent _content, FieldInfo _fieldInfo, Type _fieldType, object _value)
        {
            if (Utility_Attribute.TryGetFieldInfoAttribute(_fieldInfo, out FieldAttribute att)
                || Utility_Attribute.TryGetTypeAttribute(_fieldType, out att))
            {
                FieldDrawer objectDrawer;
                if ((objectDrawer = ObjectDrawerUtility.GetObjectDrawer(att)) != null)
                {
                    objectDrawer.Value = _value;
                    objectDrawer.FieldInfo = _fieldInfo;
                    objectDrawer.OnGUI(_content);
                    return _value;
                }
            }

            if (_fieldType.Equals(typeof(int)))
            {
                return EditorGUILayout.IntField(_content, _value == null ? 0 : (int)_value, EditorStylesExtension.NumberFieldStyle);
            }
            if (_fieldType.Equals(typeof(float)))
            {
                return EditorGUILayout.FloatField(_content, _value == null ? 0 : (float)_value, EditorStylesExtension.NumberFieldStyle);
            }
            if (_fieldType.Equals(typeof(double)))
            {
                return EditorGUILayout.DoubleField(_content, Convert.ToSingle(_value == null ? 0 : (double)_value), EditorStylesExtension.NumberFieldStyle);
            }
            if (_fieldType.Equals(typeof(long)))
            {
                return (long)EditorGUILayout.IntField(_content, Convert.ToInt32(_value == null ? 0 : (long)_value), EditorStylesExtension.NumberFieldStyle);
            }
            if (_fieldType.Equals(typeof(bool)))
            {
                return EditorGUILayout.Toggle(_content, _value == null ? false : (bool)_value);
            }
            if (_fieldType.Equals(typeof(string)))
            {
                return EditorGUILayout.TextField(_content, _value == null ? "" : (string)_value, EditorStylesExtension.TextFieldStyle);
            }
            if (_fieldType.Equals(typeof(byte)))
            {
                return Convert.ToByte(EditorGUILayout.IntField(_content, Convert.ToInt32(_value == null ? 0 : (byte)_value)));
            }
            if (_fieldType.Equals(typeof(Vector2)))
            {
                return EditorGUILayout.Vector2Field(_content, _value == null ? Vector2.zero : (Vector2)_value);
            }
            if (_fieldType.Equals(typeof(Vector2Int)))
            {
                return EditorGUILayout.Vector2IntField(_content, _value == null ? Vector2Int.zero : (Vector2Int)_value);
            }
            if (_fieldType.Equals(typeof(Vector3)))
            {
                return EditorGUILayout.Vector3Field(_content, _value == null ? Vector3.zero : (Vector3)_value);
            }
            if (_fieldType.Equals(typeof(Vector3Int)))
            {
                return EditorGUILayout.Vector3IntField(_content, _value == null ? Vector3Int.zero : (Vector3Int)_value);
            }
            if (_fieldType.Equals(typeof(Vector4)))
            {
                return EditorGUILayout.Vector4Field(_content.text, _value == null ? Vector4.zero : (Vector4)_value);
            }
            if (_fieldType.Equals(typeof(Quaternion)))
            {
                Quaternion quaternion = _value == null ? Quaternion.identity : (Quaternion)_value;
                Vector4 vector = Vector4.zero;
                vector.Set(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
                vector = EditorGUILayout.Vector4Field(_content.text, vector);
                quaternion.Set(vector.x, vector.y, vector.z, vector.w);
                return quaternion;
            }
            if (_fieldType.Equals(typeof(Color)))
            {
                return EditorGUILayout.ColorField(_content, _value == null ? Color.black : (Color)_value);
            }
            if (_fieldType.Equals(typeof(Rect)))
            {
                return EditorGUILayout.RectField(_content, _value == null ? Rect.zero : (Rect)_value);
            }
            if (_fieldType.Equals(typeof(Gradient)))
            {
                return EditorGUILayout.GradientField(_content, _value == null ? new Gradient() : (Gradient)_value);
            }
            if (_fieldType.Equals(typeof(Matrix4x4)))
            {
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                if (EditorGUILayoutExtension.DrawFoldout(_content.text.GetHashCode(), _content))
                {
                    EditorGUI.indentLevel++;
                    Matrix4x4 matrix4x = _value == null ? Matrix4x4.identity : (Matrix4x4)_value;
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            EditorGUI.BeginChangeCheck();
                            matrix4x[i, j] = EditorGUILayout.FloatField("E" + i.ToString() + j.ToString(), matrix4x[i, j]);
                            if (EditorGUI.EndChangeCheck())
                            {
                                GUI.changed = true;
                            }
                        }
                    }
                    _value = matrix4x;
                    EditorGUI.indentLevel--;
                }
                GUILayout.EndVertical();
                return _value;
            }
            if (_fieldType.Equals(typeof(AnimationCurve)))
            {
                if (_value == null)
                {
                    _value = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
                    GUI.changed = true;
                }
                return EditorGUILayout.CurveField(_content, (AnimationCurve)_value);
            }
            if (_fieldType.Equals(typeof(LayerMask)))
            {
                return EditorGUILayoutExtension.DrawLayerMask(_content, (LayerMask)_value);
            }
            if (typeof(UnityObject).IsAssignableFrom(_fieldType))
            {
                return EditorGUILayout.ObjectField(_content, (UnityObject)_value, _fieldType, true);
            }
            if (_fieldType.IsEnum)
            {
                return EditorGUILayout.EnumPopup(_content, (Enum)_value);
            }
            if (_fieldType.IsClass || (_fieldType.IsValueType && !_fieldType.IsPrimitive))
            {
                if (typeof(Delegate).IsAssignableFrom(_fieldType)) return null;
                if (typeof(object).IsAssignableFrom(_fieldType) && _value == null) return null;
                int hashCode = _value.GetHashCode();
                if (EditorGUILayoutExtension.drawnObjects.Contains(hashCode)) return null;
                try
                {
                    EditorGUILayoutExtension.drawnObjects.Add(hashCode);
                    GUILayout.BeginVertical();
                    if (_value == null)
                    {
                        if (_fieldType.IsGenericType && _fieldType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            _fieldType = Nullable.GetUnderlyingType(_fieldType);
                        _value = Activator.CreateInstance(_fieldType, true);
                    }
                    if (EditorGUILayoutExtension.DrawFoldout(hashCode, _content))
                    {
                        EditorGUI.indentLevel++;
                        _value = EditorGUILayoutExtension.DrawFields(_value);
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayoutExtension.drawnObjects.Remove(hashCode);
                    GUILayout.EndVertical();
                    return _value;
                }
                catch (Exception)
                {
                    GUILayout.EndVertical();
                    EditorGUILayoutExtension.drawnObjects.Remove(hashCode);
                    return null;
                }
            }
            EditorGUILayout.LabelField("Unsupported Type: " + _fieldType);
            return null;
        }

        private static LayerMask DrawLayerMask(GUIContent guiContent, LayerMask layerMask)
        {
            if (EditorGUILayoutExtension.layerNames == null)
            {
                EditorGUILayoutExtension.InitLayers();
            }
            int num = 0;
            for (int i = 0; i < EditorGUILayoutExtension.layerNames.Length; i++)
            {
                if ((layerMask.value & EditorGUILayoutExtension.maskValues[i]) == EditorGUILayoutExtension.maskValues[i])
                {
                    num |= 1 << i;
                }
            }
            int num2 = EditorGUILayout.MaskField(guiContent, num, EditorGUILayoutExtension.layerNames);
            if (num2 != num)
            {
                num = 0;
                for (int j = 0; j < EditorGUILayoutExtension.layerNames.Length; j++)
                {
                    if ((num2 & 1 << j) != 0)
                    {
                        num |= EditorGUILayoutExtension.maskValues[j];
                    }
                }
                layerMask.value = num;
            }
            return layerMask;
        }

        private static void InitLayers()
        {
            List<string> list = new List<string>();
            List<int> list2 = new List<int>();
            for (int i = 0; i < 32; i++)
            {
                string text = LayerMask.LayerToName(i);
                if (!string.IsNullOrEmpty(text))
                {
                    list.Add(text);
                    list2.Add(1 << i);
                }
            }
            EditorGUILayoutExtension.layerNames = list.ToArray();
            EditorGUILayoutExtension.maskValues = list2.ToArray();
        }

        private const string c_EditorPrefsFoldoutKey = "CZToolKit.Core.Editors.Foldout.";

        private static int currentKeyboardControl = -1;

        private static bool editingArray = false;

        private static int savedArraySize = -1;

        private static int editingFieldHash;

        private static HashSet<int> drawnObjects = new HashSet<int>();

        private static string[] layerNames;

        private static int[] maskValues;
    }
}
