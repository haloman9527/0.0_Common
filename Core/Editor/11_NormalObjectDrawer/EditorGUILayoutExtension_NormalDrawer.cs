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
using System.Reflection;
using UnityEditor;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace CZToolKit.Core.Editors
{
    public static partial class EditorGUILayoutExtension
    {
        /// <summary> 不是<see cref="private"/>、或者标记了<see cref="SerializeField"/>特性，并且没有标记<see cref="NonSerializedAttribute"/>特性，并且没有标记<see cref="HideInInspector"/>特性。 </summary>
        /// <returns> 满足以上条件返回<see cref="true"/> </returns>
        public static bool CanDraw(FieldInfo fieldInfo)
        {
            return ((!fieldInfo.IsPrivate && !fieldInfo.IsFamily) || Util_Attribute.TryGetFieldAttribute(fieldInfo, out SerializeField serAtt))
                    && !Util_Attribute.TryGetTypeAttribute(fieldInfo.DeclaringType, out NonSerializedAttribute nonAtt)
                    && !Util_Attribute.TryGetFieldAttribute(fieldInfo, out HideInInspector hideAtt);
        }

        public static bool DrawFoldout(int hash, GUIContent guiContent)
        {
            string text = string.Concat(c_EditorPrefsFoldoutKey, hash, ".", guiContent.text);
            var @bool = GUIHelper.GetContextData(text, false);
            @bool.value = EditorGUILayout.Foldout(@bool.value, guiContent, true);
            return @bool.value;
        }

        public static void DrawField(FieldInfo fieldInfo, object context, GUIContent label)
        {
            object value = fieldInfo.GetValue(context);

            // 判断是否是数组
            if (typeof(IList).IsAssignableFrom(fieldInfo.FieldType))
                value = DrawArrayField(fieldInfo.FieldType, value, label);
            else
                value = DrawField(context, label);

            value = DrawField(value, label);
            fieldInfo.SetValue(context, value);
        }

        public static void DrawField(FieldInfo fieldInfo, object context, string label)
        {
            DrawField(fieldInfo, context, GUIHelper.TextContent(label));
        }

        public static void DrawField(FieldInfo fieldInfo, object context)
        {
            GUIContent label = null;
            if (Util_Attribute.TryGetFieldAttribute(fieldInfo, out TooltipAttribute tooltipAtt))
                label = GUIHelper.TextContent(ObjectNames.NicifyVariableName(fieldInfo.Name), tooltipAtt.tooltip);
            else
                label = GUIHelper.TextContent(ObjectNames.NicifyVariableName(fieldInfo.Name));
            DrawField(fieldInfo, context, label);
        }

        public static object DrawField(object value, GUIContent label)
        {
            Type type = value.GetType();

            if (type.IsClass || (type.IsValueType && !type.IsPrimitive))
            {
                if (typeof(Delegate).IsAssignableFrom(type)) return null;
                if (typeof(object).IsAssignableFrom(type) && value == null) return null;
                int hashCode = value.GetHashCode();

                if (value == null)
                {
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        type = Nullable.GetUnderlyingType(type);
                    value = Activator.CreateInstance(type, true);
                }

                GUILayout.BeginVertical();
                if (DrawFoldout(hashCode, label))
                {
                    EditorGUI.indentLevel++;
                    foreach (var field in Util_Reflection.GetFieldInfos(type))
                    {
                        if (!CanDraw(field)) continue;

                        DrawField(field, value, label);
                    }
                    EditorGUI.indentLevel--;
                }
                GUILayout.EndVertical();
                return value;
            }

            float height = EditorGUIExtension.GetHeight(type, label);
            return EditorGUIExtension.DrawField(EditorGUILayout.GetControlRect(true, height), value, label);

            //if (_fieldType.Equals(typeof(Matrix4x4)))
            //{
            //    GUILayout.BeginVertical(new GUILayoutOption[0]);
            //    if (EditorGUILayoutExtension.DrawFoldout(label.text.GetHashCode(), label))
            //    {
            //        EditorGUI.indentLevel++;
            //        Matrix4x4 matrix4x = _value == null ? Matrix4x4.identity : (Matrix4x4)_value;
            //        for (int i = 0; i < 4; i++)
            //        {
            //            for (int j = 0; j < 4; j++)
            //            {
            //                EditorGUI.BeginChangeCheck();
            //                matrix4x[i, j] = EditorGUILayout.FloatField("E" + i.ToString() + j.ToString(), matrix4x[i, j]);
            //                if (EditorGUI.EndChangeCheck())
            //                {
            //                    GUI.changed = true;
            //                }
            //            }
            //        }
            //        _value = matrix4x;
            //        EditorGUI.indentLevel--;
            //    }
            //    GUILayout.EndVertical();
            //    return _value;
            //}
        }

        public static object DrawField(object value, string label)
        {
            return DrawField(value, GUIHelper.TextContent(label));
        }

        public static object DrawField(Type type, object value, GUIContent label)
        {
            return EditorGUIExtension.DrawField(EditorGUILayout.GetControlRect(true, EditorGUIExtension.GetHeight(type, label)), type, value, label);
        }

        public static object DrawField(Type type, object value, string label)
        {
            return DrawField(type, value, GUIHelper.TextContent(label));
        }

        static object DrawArrayField(Type fieldType, object value, GUIContent label)
        {
            Type elementType;
            if (fieldType.IsArray)
                elementType = fieldType.GetElementType();
            else
            {
                Type type2 = fieldType;
                while (!type2.IsGenericType)
                {
                    type2 = type2.BaseType;
                }
                elementType = type2.GetGenericArguments()[0];
            }

            IList list;
            if (value == null)
            {
                if (fieldType.IsGenericType || fieldType.IsArray)
                {
                    value = list = (Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[]
                    {
                        elementType
                    }), true) as IList);
                }
                else
                {
                    value = list = (Activator.CreateInstance(fieldType, true) as IList);
                }
                if (fieldType.IsArray)
                {
                    Array array = Array.CreateInstance(elementType, list.Count);
                    list.CopyTo(array, 0);
                    value = list = array;
                }
                GUI.changed = true;
            }
            else
            {
                list = (IList)value;
            }
            if (DrawFoldout(value.GetHashCode(), label))
            {
                EditorGUI.indentLevel++;
                bool flag = value.GetHashCode() == editingFieldHash;
                int count = (!flag) ? list.Count : savedArraySize;
                int newCount = EditorGUILayout.IntField(GUIHelper.TextContent("Size"), count);
                if (flag && editingArray && (GUIUtility.keyboardControl != currentKeyboardControl || (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return)))
                {
                    if (newCount != list.Count)
                    {
                        int currentCount = list.Count;
                        if (newCount > currentCount)
                        {
                            if (fieldType.IsArray)
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
                            if (!fieldType.IsArray)
                            {
                                while (list.Count > newCount)
                                {
                                    list.RemoveAt(list.Count - 1);
                                }
                            }
                        }
                    }
                    editingArray = false;
                    savedArraySize = -1;
                    editingFieldHash = -1;
                    GUI.changed = true;
                }
                else if (newCount != count)
                {
                    if (!editingArray)
                    {
                        currentKeyboardControl = GUIUtility.keyboardControl;
                        editingArray = true;
                        editingFieldHash = value.GetHashCode();
                    }
                    savedArraySize = newCount;
                }

                for (int k = 0; k < list.Count; k++)
                {
                    label.text = "Element " + k;
                    list[k] = DrawField(list[k], label);
                }

                EditorGUI.indentLevel--;
            }
            return list;
        }

        const string c_EditorPrefsFoldoutKey = "CZToolKit.Core.Editors.Foldout.";

        static int currentKeyboardControl = -1;

        static bool editingArray = false;

        static int savedArraySize = -1;

        static int editingFieldHash;
    }
}
#endif