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
    public static partial class EditorGUILayoutExtension
    {
        public static void PropertyField(SerializedPropertyS property)
        {
            var height = EditorGUIUtility.singleLineHeight;
            if (property.drawer != null)
            {
                height = EditorGUIExtension.GetPropertyHeight(property);
                var rect = EditorGUILayout.GetControlRect(GUILayout.Height(height));
                property.drawer.OnGUI(rect, property.niceName);
            }
            else if (property.hasChildren)
            {
                if (property.isArray)
                {
                    var rect = EditorGUILayout.GetControlRect(GUILayout.Height(height));
                    property.expanded = EditorGUI.Foldout(rect, property.expanded, property.niceName);

                    if (property.Value == null)
                        property.Value = EditorGUIExtension.CreateInstance(property.propertyType);
                    IList list = (IList)property.Value;
                    if (property.expanded)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUI.BeginChangeCheck();
                        rect = EditorGUILayout.GetControlRect(GUILayout.Height(height));
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
                                var newList = (Activator.CreateInstance(property.propertyType, true) as IList);
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
                                            list.Add(Activator.CreateInstance(type, true));
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
                                Array array = Array.CreateInstance(elementType, list.Count);
                                list.CopyTo(array, 0);
                                list = array;
                                property.Value = list;
                            }
                            GUI.changed = true;
                        }
                        foreach (var element in property.GetIterator())
                        {
                            PropertyField(element);
                        }
                        EditorGUI.indentLevel--;
                    }
                }
                else
                {
                    var rect = EditorGUILayout.GetControlRect(GUILayout.Height(height));
                    property.expanded = EditorGUI.Foldout(rect, property.expanded, property.niceName);
                    if (property.expanded)
                    {
                        EditorGUI.indentLevel++;
                        foreach (var children in property.GetIterator())
                        {
                            PropertyField(children);
                        }
                        EditorGUI.indentLevel--;
                    }
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                var value = EditorGUILayoutExtension.DrawField(property.propertyType, property.Value, property.niceName);
                if (EditorGUI.EndChangeCheck())
                {
                    property.Value = value;
                }
            }
        }
    }
}
#endif