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
 *  Blog: https://www.mindgear.net/
 *
 */

#endregion

using System;
using System.Collections.Generic;
using CZToolKit;
using CZToolKitEditor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace CZToolKitEditor
{
    [CustomEditor(typeof(ReferenceCollector))]
    public class ReferenceCollectorInspector : Editor
    {
        public static class Styles
        {
            public static readonly GUIContent ClearLabel = new GUIContent(EditorGUIUtility.FindTexture("Refresh"), "Clear All");
            public static readonly GUIContent CleanUpLabel = new GUIContent(EditorGUIUtility.FindTexture("UnityEditor.ConsoleWindow"), "Clear Empty");
            public static readonly GUIContent CleanUpDuplicateLabel = new GUIContent("C", "Clear Duplicate");
            public static readonly GUIContent SortLabel = new GUIContent(EditorGUIUtility.FindTexture("AlphabeticalSorting"), "Sort");
            public static readonly GUIContent ComponentsLabel = new GUIContent(EditorGUIUtility.FindTexture("UnityEditor.HierarchyWindow"), "Components");
        }

        private ReorderableList referencesList;

        private void OnEnable()
        {
            var referenceCollector = serializedObject.targetObject as ReferenceCollector;
            referencesList = new ReorderableList(serializedObject, serializedObject.FindProperty("references"), true, true, true, true);
            referencesList.drawHeaderCallback = (a) =>
            {
                GUI.Label(a, "References");

                var clearButtonRect = new Rect(a.x + a.width - 30, a.y, 30, a.height);
                if (GUI.Button(clearButtonRect, Styles.ClearLabel, EditorStyles.toolbarButton))
                {
                    Undo.RecordObject(referenceCollector, "Clear All");
                    referenceCollector.references.Clear();
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.UpdateIfRequiredOrScript();
                }

                var cleanButtonRect = new Rect(a.x + a.width - 60, a.y, 30, a.height);
                if (GUI.Button(cleanButtonRect, Styles.CleanUpLabel, EditorStyles.toolbarButton))
                {
                    Undo.RecordObject(referenceCollector, "Clear Empty");
                    referenceCollector.references.RemoveAll(pair => string.IsNullOrEmpty(pair.key) || pair.value == null);
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.UpdateIfRequiredOrScript();
                }
                
                var clearSameButtonRect = new Rect(a.x + a.width - 90, a.y, 30, a.height);
                if (GUI.Button(clearSameButtonRect, Styles.CleanUpDuplicateLabel, EditorStyles.toolbarButton))
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
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.UpdateIfRequiredOrScript();
                }

                var sortButtonRect = new Rect(a.x + a.width - 120, a.y, 30, a.height);
                if (GUI.Button(sortButtonRect, Styles.SortLabel, EditorStyles.toolbarButton))
                {
                    Undo.RecordObject(referenceCollector, "Sort ReferenceData");
                    referenceCollector.references.QuickSort((l, r) => { return String.Compare(l.key, r.key, StringComparison.Ordinal); });
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.UpdateIfRequiredOrScript();
                }

            };
            referencesList.drawElementCallback += (a, b, c, d) =>
            {
                var element = referencesList.serializedProperty.GetArrayElementAtIndex(b);
                var key = element.FindPropertyRelative("key");
                var value = element.FindPropertyRelative("value");

                var keyFieldRect = new Rect(a.x, a.y, a.width * 0.3f - 1, a.height);
                var objFieldRect = new Rect(a.x + a.width * 0.3f + 1, a.y, a.width * 0.7f - 26, a.height);
                var dropDownButtonRect = new Rect(a.xMax - 25, a.y, 25, a.height);

                EditorGUI.BeginChangeCheck();
                var sourceK = key.stringValue;
                var k = EditorGUI.DelayedTextField(keyFieldRect, sourceK);
                if (!string.IsNullOrEmpty(k) && k != sourceK && !referenceCollector.ReferencesMap.ContainsKey(k))
                    key.stringValue = k;

                var sourceV = value.objectReferenceValue;
                EditorGUI.PropertyField(objFieldRect, value, GUIContent.none);
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.UpdateIfRequiredOrScript();
                }

                EditorGUI.BeginDisabledGroup(!(value.objectReferenceValue is GameObject) && !(value.objectReferenceValue is Component));
                if (GUI.Button(dropDownButtonRect, Styles.ComponentsLabel, EditorStyles.toolbarButton))
                {
                    GenericMenu menu = new GenericMenu();
                    Component[] components = null;
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
                        Undo.RecordObject(referenceCollector, "Change Component");
                        value.objectReferenceValue = gameObject;
                        serializedObject.ApplyModifiedProperties();
                        serializedObject.UpdateIfRequiredOrScript();
                    });
                    for (int i = 0; i < components.Length; i++)
                    {
                        var component = components[i];
                        menu.AddItem(EditorGUIUtility.TrTextContent($"{i + 1}:{component.GetType().Name}"), false, () =>
                        {
                            Undo.RecordObject(referenceCollector, "Change Component");
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
                } while (referenceCollector.ReferencesMap.ContainsKey(key));
                referenceCollector.references.Add(new ReferenceCollector.ReferencePair() { key = key, value = null });
                serializedObject.ApplyModifiedProperties();
                serializedObject.UpdateIfRequiredOrScript();
            };
            referencesList.onRemoveCallback += (a) =>
            {
                Undo.RecordObject(referenceCollector, "Remove ReferenceData");
                referenceCollector.references.RemoveAt(referencesList.index);
                serializedObject.ApplyModifiedProperties();
                serializedObject.UpdateIfRequiredOrScript();
                referencesList.index = Mathf.Clamp(referencesList.index, 0, referencesList.count - 1);
            };
        }

        public override void OnInspectorGUI()
        {
            var evt = Event.current;

            base.OnInspectorGUI();
            var rect = EditorGUILayout.BeginVertical();
            referencesList.DoLayoutList();
            EditorGUILayout.EndVertical();

            var results = EditorGUIExtension.DragDropAreaMulti(rect, DragAndDropVisualMode.Generic);
            if (results != null)
            {
                var referenceCollector = serializedObject.targetObject as ReferenceCollector;
                foreach (var obj in results)
                {
                    string key = string.Empty;
                    do
                    {
                        key = UnityEngine.Random.Range(int.MinValue, int.MaxValue).ToString();
                    } while (referenceCollector.ReferencesMap.ContainsKey(key));
                    Undo.RecordObjects(targets, "Add ReferenceData");   
                    referenceCollector.references.Add(new ReferenceCollector.ReferencePair() { key = key, value = obj });
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.UpdateIfRequiredOrScript();
                }
            }
        }
        
    }
}