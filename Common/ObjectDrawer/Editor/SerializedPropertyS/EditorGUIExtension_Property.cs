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
using UnityEditor;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace CZToolKit.Core.Editors
{
    public static partial class EditorGUIExtension
    {
        public static float GetPropertyHeight(SerializedPropertyS property)
        {
            return GetPropertyHeight(property, property.expanded, property.niceName);
        }

        public static float GetPropertyHeight(SerializedPropertyS property, GUIContent label)
        {
            return GetPropertyHeight(property, property.expanded, label);
        }

        public static float GetPropertyHeight(SerializedPropertyS property, bool includeChildren)
        {
            return GetPropertyHeight(property, includeChildren, property.niceName);
        }

        public static float GetPropertyHeight(SerializedPropertyS property, bool includeChildren, GUIContent label)
        {
            if (property.drawer != null)
                return property.drawer.GetHeight(property.niceName);

            if (IsBasicType(property.propertyType))
                return GetHeight(property.propertyType, label);

            float height = EditorGUIUtility.singleLineHeight;
            if (includeChildren && property.expanded)
            {
                foreach (var children in property.GetIterator())
                {
                    height += GetPropertyHeight(children) + EditorGUIUtility.standardVerticalSpacing;
                }
                if (property.isArray)
                {
                    height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }
            }
            return height;
        }

        public static void PropertyField(Rect rect, SerializedPropertyS property)
        {
            if (property.drawer != null)
            {
                property.drawer.OnGUI(rect, property.niceName);
            }
            else if (property.hasChildren)
            {
                if (property.isArray)
                {
                    rect.height = EditorGUIUtility.singleLineHeight;
                    property.expanded = EditorGUI.Foldout(rect, property.expanded, property.niceName);

                    if (property.Value == null)
                        property.Value = EditorGUIExtension.CreateInstance(property.propertyType);
                    IList list = (IList)property.Value;
                    if (property.expanded)
                    {
                        EditorGUI.indentLevel++;

                        EditorGUI.BeginChangeCheck();
                        rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                        rect.height = EditorGUIUtility.singleLineHeight;
                        var size = EditorGUI.DelayedIntField(rect, GUIHelper.TextContent("Size"), list.Count);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Type elementType;
                            if (property.propertyType.IsArray)
                                elementType = property.propertyType.GetElementType();
                            else
                            {
                                Type tmpType = property.propertyType;
                                while (!tmpType.IsGenericType)
                                {
                                    tmpType = tmpType.BaseType;
                                }
                                elementType = tmpType.GetGenericArguments()[0];
                            }

                            if (!property.propertyType.IsGenericType && !property.propertyType.IsArray)
                            {
                                var newList = (EditorGUIExtension.CreateInstance(property.propertyType) as IList);
                                list = newList;
                            }

                            int currentCount = list.Count;
                            if (size > currentCount)
                            {
                                if (!property.propertyType.IsArray)
                                {
                                    Type type = list.Count > 0 ? list[list.Count - 1].GetType() : elementType;
                                    if (!typeof(UnityObject).IsAssignableFrom(type))
                                    {
                                        for (int i = currentCount; i < size; i++)
                                            list.Add(EditorGUIExtension.CreateInstance(type));
                                    }
                                    else
                                    {
                                        for (int i = currentCount; i < size; i++)
                                            list.Add(null);
                                    }
                                }
                            }
                            else
                            {
                                if (!property.propertyType.IsArray)
                                {
                                    while (list.Count > size)
                                    {
                                        list.RemoveAt(list.Count - 1);
                                    }
                                }
                            }

                            if (property.propertyType.IsArray)
                            {
                                Array array = Array.CreateInstance(elementType, size);
                                if (size > list.Count)
                                {
                                    list.CopyTo(array, 0);
                                }
                                else
                                {
                                    for (int i = 0; i < size; i++)
                                    {
                                        array.SetValue(list[i], i);
                                    }
                                }
                                property.Value = list = array;
                            }
                            GUI.changed = true;
                        }
                        foreach (var element in property.GetIterator())
                        {
                            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                            rect.height = GetPropertyHeight(element, false);
                            PropertyField(rect, element);
                        }
                        EditorGUI.indentLevel--;
                    }
                }
                else
                {
                    rect.height = EditorGUIUtility.singleLineHeight;
                    property.expanded = EditorGUI.Foldout(rect, property.expanded, property.niceName);
                    if (property.expanded)
                    {
                        EditorGUI.indentLevel++;
                        foreach (var children in property.GetIterator())
                        {
                            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                            rect.height = GetPropertyHeight(children, false);
                            PropertyField(rect, children);
                        }
                        EditorGUI.indentLevel--;
                    }
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                var value = DrawField(rect, property.propertyType, property.Value, property.niceName);
                if (EditorGUI.EndChangeCheck())
                {
                    property.Value = value;
                }
            }
        }
    }
}
#endif