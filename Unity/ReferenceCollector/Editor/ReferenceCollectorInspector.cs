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
 *  Github: https://github.com/haloman9527
 *  Blog: https://www.haloman.net/
 *
 */

#endregion

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using JiangeEditor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Jiange
{
    [CustomEditor(typeof(ReferenceCollector))]
    public class UIPanelDataInspector : Editor
    {
        public static class Styles
        {
            public static readonly GUIContent ClearSameLabel = new GUIContent("S", "相同值只留第一个");
            public static readonly GUIContent ClearEmptyLabel = new GUIContent("E", "清理所有空值");
            public static readonly GUIContent ClearAllLabel = new GUIContent("C", "清空所有");
            public static readonly GUIContent SortLabel = new GUIContent(EditorGUIUtility.FindTexture("AlphabeticalSorting"), "排序");
            public static readonly GUIContent ComponentsLabel = new GUIContent(EditorGUIUtility.FindTexture("UnityEditor.HierarchyWindow"), "Components");
        }

        private ReorderableList referencesList;

        protected void OnEnable()
        {
            var referenceCollector = (ReferenceCollector)target;
            referencesList = new ReorderableList(serializedObject, serializedObject.FindProperty("references"), true, true, true, true);
            referencesList.elementHeight = 20;
            referencesList.drawHeaderCallback = (a) =>
            {
                GUI.Label(a, "References");

                var clearButtonRect = new Rect(a.x + a.width - 30, a.y, 30, a.height);
                if (GUI.Button(clearButtonRect, Styles.ClearAllLabel, EditorStyles.toolbarButton))
                {
                    Undo.RecordObject(referenceCollector, "Clear All");
                    referenceCollector.references.Clear();
                }

                var cleanButtonRect = new Rect(a.x + a.width - 60, a.y, 30, a.height);
                if (GUI.Button(cleanButtonRect, Styles.ClearEmptyLabel, EditorStyles.toolbarButton))
                {
                    Undo.RecordObject(referenceCollector, "Clear Empty");
                    referenceCollector.references.RemoveAll(pair => string.IsNullOrEmpty(pair.key) || pair.value == null);
                }

                var clearSameButtonRect = new Rect(a.x + a.width - 90, a.y, 30, a.height);
                if (GUI.Button(clearSameButtonRect, Styles.ClearSameLabel, EditorStyles.toolbarButton))
                {
                    Undo.RecordObject(referenceCollector, "Clear Duplicates");
                    var set = new HashSet<UnityObject>();
                    for (int i = 0; i < referenceCollector.references.Count; i++)
                    {
                        var obj = referenceCollector.references[i].value;
                        if (obj == null)
                            continue;
                        if (set.Add(obj))
                            continue;
                        referenceCollector.references.RemoveAt(i);
                        i--;
                    }
                }

                var sortButtonRect = new Rect(a.x + a.width - 120, a.y, 30, a.height);
                if (GUI.Button(sortButtonRect, Styles.SortLabel, EditorStyles.toolbarButton))
                {
                    Undo.RecordObject(referenceCollector, "Sort ReferenceData");
                    referenceCollector.references.QuickSort((l, r) => { return String.Compare(l.key, r.key, StringComparison.Ordinal); });
                }
            };
            referencesList.drawElementCallback += (a, b, c, d) =>
            {
                var element = referencesList.serializedProperty.GetArrayElementAtIndex(b);
                var key = element.FindPropertyRelative("key");
                var value = element.FindPropertyRelative("value");

                var keyFieldRect = new Rect(a.x, a.y + 1, a.width * 0.3f - 1, a.height - 2);
                var objFieldRect = new Rect(a.x + a.width * 0.3f + 1, a.y + 1, a.width * 0.7f - 26, a.height - 2);
                var dropDownButtonRect = new Rect(a.xMax - 25, a.y + 1, 25, a.height + 1);

                var sourceK = key.stringValue;
                var k = EditorGUI.DelayedTextField(keyFieldRect, sourceK);
                if (!string.IsNullOrEmpty(k) && k != sourceK && !referenceCollector.Contains(k))
                    key.stringValue = k;

                EditorGUI.PropertyField(objFieldRect, value, GUIContent.none);

                EditorGUI.BeginDisabledGroup(!(value.objectReferenceValue is GameObject) && !(value.objectReferenceValue is Component));
                if (GUI.Button(dropDownButtonRect, Styles.ComponentsLabel, EditorStyles.toolbarButton))
                {
                    GenericMenu menu = new GenericMenu();
                    Component[] components = Array.Empty<Component>();
                    GameObject gameObject = null;
                    switch (value.objectReferenceValue)
                    {
                        case GameObject _gameObject:
                            components = _gameObject.GetComponents<Component>();
                            gameObject = _gameObject;
                            break;
                        case Component _component:
                            components = _component.GetComponents<Component>();
                            gameObject = _component.gameObject;
                            break;
                    }

                    menu.AddItem(EditorGUIUtility.TrTextContent($"0:GameObject"), false, () =>
                    {
                        value.objectReferenceValue = gameObject;
                        serializedObject.ApplyModifiedProperties();
                        serializedObject.UpdateIfRequiredOrScript();
                    });
                    
                    for (int i = 0; i < components.Length; i++)
                    {
                        var component = components[i];
                        menu.AddItem(EditorGUIUtility.TrTextContent($"{i + 1}:{component.GetType().Name}"), false, () =>
                        {
                            value.objectReferenceValue = component;
                            serializedObject.ApplyModifiedProperties();
                            serializedObject.UpdateIfRequiredOrScript();
                        });
                    }

                    menu.DropDown(dropDownButtonRect);
                }

                EditorGUI.EndDisabledGroup();
            };
            referencesList.onAddCallback += (list) =>
            {
                Undo.RecordObject(referenceCollector, "Add ReferenceData");
                string key = string.Empty;
                do
                {
                    key = UnityEngine.Random.Range(int.MinValue, int.MaxValue).ToString();
                } while (referenceCollector.Contains(key));

                referenceCollector.Set(key, null);
            };
            referencesList.onRemoveCallback += (a) =>
            {
                var serializedProperty = serializedObject.FindProperty("references");
                serializedProperty.DeleteArrayElementAtIndex(referencesList.index);
                referencesList.index = Mathf.Clamp(referencesList.index, 0, referencesList.count - 1);
            };
        }

        public override void OnInspectorGUI()
        {
            var rect = EditorGUILayout.BeginVertical();
            referencesList.DoLayoutList();
            EditorGUILayout.EndVertical();

            var results = EditorGUIExtension.DragDropAreaMulti(rect, DragAndDropVisualMode.Generic);
            if (results != null)
            {
                var referenceCollector = (ReferenceCollector)target;
                foreach (var obj in results)
                {
                    string key = obj.name;
                    while (referenceCollector.Contains(key))
                    {
                        key = UnityEngine.Random.Range(int.MinValue, int.MaxValue).ToString();
                    }

                    Undo.RecordObjects(targets, "Add ReferenceData");
                    referenceCollector.Set(key, obj);
                }
            }

            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
                serializedObject.UpdateIfRequiredOrScript();
            }
        }
    }
}
#endif