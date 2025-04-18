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
using Atom.UnityEditors;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Atom
{
    [CustomEditor(typeof(ReferenceCollector))]
    public class UIPanelDataInspector : Editor
    {
        public static class Styles
        {
            private static GUIStyle s_AlignRightLabel;

            public static GUIStyle AlignRightLabel
            {
                get
                {
                    if (s_AlignRightLabel == null)
                    {
                        s_AlignRightLabel = new GUIStyle(EditorStyles.label);
                        s_AlignRightLabel.alignment = TextAnchor.MiddleRight;
                    }

                    return s_AlignRightLabel;
                }
            }

            public static readonly GUIContent ModeLabel = new GUIContent("M", "切换模式");
            public static readonly GUIContent ClearSameLabel = new GUIContent("S", "相同值只留第一个");
            public static readonly GUIContent ClearEmptyLabel = new GUIContent("E", "清理所有空值");
            public static readonly GUIContent ClearAllLabel = new GUIContent("C", "清空所有");
            public static readonly GUIContent SortLabel = new GUIContent(EditorGUIUtility.FindTexture("AlphabeticalSorting"), "排序");
            public static readonly GUIContent ComponentsLabel = new GUIContent(EditorGUIUtility.FindTexture("UnityEditor.HierarchyWindow"), "Components");
        }

        const int PageMaxCount = 20;
        const int LineHeight = 20;

        private ReorderableList referencesList;
        public string searchText;
        public SearchField searchField;
        public float scrollPosition;
        public bool drawWithList;

        protected void OnEnable()
        {
            var referenceCollector = (ReferenceCollector)target;
            searchField = new SearchField();
            referencesList = new ReorderableList(serializedObject, serializedObject.FindProperty("references"), true, true, true, false);
            referencesList.elementHeight = 20;
            referencesList.drawHeaderCallback = (headerRect) =>
            {
                headerRect.xMax += 6;
                DrawHeader(headerRect);
            };
            referencesList.drawElementCallback += (rect, index, c, d) => { DrawElement(rect, referencesList.serializedProperty, index); };
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
        }

        public override void OnInspectorGUI()
        {
            var references = serializedObject.FindProperty("references");
            var rect = EditorGUILayout.GetControlRect(false, 50);
            GUI.Box(rect, GUIContent.none, "FrameBox");
            var results = EditorGUIExtension.DragDropAreaMulti(rect, DragAndDropVisualMode.Generic);

            GUI.Label(rect, "拖拽到此处添加", EditorStyles.centeredGreyMiniLabel);
            if (results != null)
            {
                var referenceCollector = (ReferenceCollector)target;
                foreach (var obj in results)
                {
                    var key = obj.name;
                    var index = 0;
                    while (referenceCollector.Contains(key))
                    {
                        key = $"{obj.name}_{index++}";
                    }

                    Undo.RecordObjects(targets, "Add ReferenceData");
                    referenceCollector.Set(key, obj);
                }

                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }

            if (drawWithList)
            {
                referencesList.DoLayoutList();
            }
            else
            {
                DrawScrollList(references, results);
            }

            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
                serializedObject.UpdateIfRequiredOrScript();
            }
        }

        private void DrawScrollList(SerializedProperty references, UnityEngine.Object[] dragResults)
        {
            var totalCount = references.arraySize;
            if (!string.IsNullOrEmpty(searchText))
            {
                totalCount = 0;
                for (int i = 0; i < references.arraySize; i++)
                {
                    var element = references.GetArrayElementAtIndex(i);
                    var key = element.FindPropertyRelative("key");
                    var value = element.FindPropertyRelative("value");
                    var keyMatched = key.stringValue.Contains(searchText, StringComparison.OrdinalIgnoreCase);
                    var valueMatched = value.objectReferenceValue && value.objectReferenceValue.name.Contains(searchText, StringComparison.OrdinalIgnoreCase);
                    if (keyMatched || valueMatched)
                    {
                        totalCount++;
                    }
                }
            }

            if (dragResults != null && dragResults.Length > 0)
            {
                scrollPosition = Mathf.Max(0, totalCount - PageMaxCount);
            }

            GUILayout.Space(1);
            var headerRect = EditorGUILayout.GetControlRect(false);
            var headerControlWidth = DrawHeader(headerRect);
            var foldoutRect = new Rect(headerRect.x, headerRect.y, headerRect.width - headerControlWidth, headerRect.height);
            references.isExpanded = EditorGUI.Foldout(foldoutRect, references.isExpanded, $"References - ({totalCount})", true);
            if (references.isExpanded)
            {
                searchText = searchField.OnGUI(searchText);
                var listRect = EditorGUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                if (string.IsNullOrEmpty(searchText))
                {
                    var drawingCount = 0;
                    for (int i = (int)scrollPosition; i < totalCount; i++)
                    {
                        var lineRect = EditorGUILayout.GetControlRect(false, LineHeight);
                        DrawElement(lineRect, references, i);
                        drawingCount++;
                        if (drawingCount >= PageMaxCount)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    var drawingCount = 0;
                    var beginIndex = -1;

                    for (int i = 0; i < references.arraySize; i++)
                    {
                        var element = references.GetArrayElementAtIndex(i);
                        var key = element.FindPropertyRelative("key");
                        var value = element.FindPropertyRelative("value");
                        var keyMatched = key.stringValue.Contains(searchText, StringComparison.OrdinalIgnoreCase);
                        var valueMatched = value.objectReferenceValue && value.objectReferenceValue.name.Contains(searchText, StringComparison.OrdinalIgnoreCase);
                        if (!keyMatched && !valueMatched)
                        {
                            continue;
                        }

                        beginIndex++;
                        if (beginIndex < (int)scrollPosition)
                        {
                            continue;
                        }

                        var lineRect = EditorGUILayout.GetControlRect(false, LineHeight);
                        DrawElement(lineRect, references, i);
                        drawingCount++;
                        if (drawingCount >= PageMaxCount)
                        {
                            break;
                        }
                    }
                }

                // referencesList.DoLayoutList();
                GUILayout.EndVertical();

                if (totalCount > PageMaxCount)
                {
                    GUILayout.BeginVertical(GUILayout.Width(20));
                    scrollPosition = GUILayout.VerticalScrollbar(scrollPosition, PageMaxCount, 0, totalCount, GUILayout.ExpandHeight(true));
                    scrollPosition = Mathf.Round(scrollPosition);
                    if (listRect.Contains(Event.current.mousePosition) && Event.current.isScrollWheel)
                    {
                        if (Event.current.delta.y > 0)
                        {
                            scrollPosition++;
                        }
                        else if (Event.current.delta.y < 0)
                        {
                            scrollPosition--;
                        }

                        Event.current.Use();
                    }

                    GUILayout.EndVertical();
                }

                EditorGUILayout.EndHorizontal();
            }

            scrollPosition = Mathf.Clamp(scrollPosition, 0, Mathf.Max(0, totalCount - PageMaxCount));
        }

        private void DrawElement(Rect rect, SerializedProperty array, int index)
        {
            var element = array.GetArrayElementAtIndex(index);
            var referenceCollector = (ReferenceCollector)target;
            var key = element.FindPropertyRelative("key");
            var value = element.FindPropertyRelative("value");
            var controlWidth = 60;
            var indexWidth = 30;
            var controlHeight = rect.height + 1;
            var controlX = rect.xMax - controlWidth;
            var controlY = rect.y + 1;

            var indexHeight = rect.height - 2;
            var indexX = rect.x;
            var indexY = rect.y + 1;

            var fieldWidth = rect.width - controlWidth - indexWidth;
            var fieldHeight = rect.height - 2;
            var fieldX = rect.x + indexWidth;
            var fieldY = rect.y + 1;

            var indexRect = new Rect(indexX, indexY, indexWidth, indexHeight);

            var keyFieldRect = new Rect(fieldX, fieldY, fieldWidth * 0.3f - 1, fieldHeight);
            var objFieldRect = new Rect(fieldX + fieldWidth * 0.3f + 1, fieldY, fieldWidth * 0.7f, fieldHeight);

            var dropDownButtonRect = new Rect(controlX, controlY, 35, controlHeight);
            var rightRemoveButtonRect = new Rect(controlX + 35, controlY, 25, controlHeight);

            GUI.Label(indexRect, $"{index.ToString()} : ", Styles.AlignRightLabel);

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

            if (GUI.Button(rightRemoveButtonRect, EditorGUIUtility.IconContent("redLight"), EditorStyles.toolbarButton))
            {
                referencesList.serializedProperty.DeleteArrayElementAtIndex(index);
                referencesList.serializedProperty.serializedObject.ApplyModifiedProperties();
                referencesList.serializedProperty.serializedObject.UpdateIfRequiredOrScript();
                GUIUtility.ExitGUI();
            }
        }

        private float DrawHeader(Rect headerRect)
        {
            var referenceCollector = (ReferenceCollector)target;
            GUI.Label(headerRect, "References");

            var clearButtonRect = new Rect(headerRect.x + headerRect.width - 30, headerRect.y, 30, headerRect.height);
            if (GUI.Button(clearButtonRect, Styles.ClearAllLabel, EditorStyles.toolbarButton))
            {
                Undo.RecordObject(referenceCollector, "Clear All");
                referenceCollector.references.Clear();
            }

            var cleanButtonRect = new Rect(headerRect.x + headerRect.width - 60, headerRect.y, 30, headerRect.height);
            if (GUI.Button(cleanButtonRect, Styles.ClearEmptyLabel, EditorStyles.toolbarButton))
            {
                Undo.RecordObject(referenceCollector, "Clear Empty");
                referenceCollector.references.RemoveAll(pair => string.IsNullOrEmpty(pair.key) || pair.value == null);
            }

            var clearSameButtonRect = new Rect(headerRect.x + headerRect.width - 90, headerRect.y, 30, headerRect.height);
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

            var sortButtonRect = new Rect(headerRect.x + headerRect.width - 120, headerRect.y, 30, headerRect.height);
            if (GUI.Button(sortButtonRect, Styles.SortLabel, EditorStyles.toolbarButton))
            {
                Undo.RecordObject(referenceCollector, "Sort ReferenceData");
                referenceCollector.references.QuickSort((l, r) => { return String.Compare(l.key, r.key, StringComparison.Ordinal); });
            }

            var modeButtonRect = new Rect(headerRect.x + headerRect.width - 150, headerRect.y, 30, headerRect.height);
            if (GUI.Button(modeButtonRect, Styles.ModeLabel, EditorStyles.toolbarButton))
            {
                drawWithList = !drawWithList;
            }

            return 150;
        }
    }
}
#endif