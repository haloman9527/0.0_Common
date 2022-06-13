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
using UnityEditor;
using UnityEngine;
using UnityEditor.AnimatedValues;

using UnityObject = UnityEngine.Object;

namespace CZToolKit.Core.Editors
{
    public static partial class EditorGUILayoutExtension
    {
        /// <summary>  </summary>
        /// <param name="key"> 用于存取上下文数据 </param>
        /// <returns> visible or not </returns>
        public static bool BeginFadeGroup(string key, bool visible, float speed = 1)
        {
            if (!GUIHelper.TryGetContextData<AnimFloat>(key, out var contextData))
                contextData.value = new AnimFloat(visible ? 1 : 0);
            contextData.value.speed = speed * (visible ? 1 : 2);
            contextData.value.target = visible ? 1 : 0;
            float t = contextData.value.value;
            if (visible)
            {
                t--;
                t = -(t * t * t * t - 1);
            }
            else
            {
                t = t * t;
            }

            EditorGUIExtension.BeginAlpha(t);
            return EditorGUILayout.BeginFadeGroup(t);
        }

        public static void EndFadeGroup()
        {
            EditorGUILayout.EndFadeGroup();
            EditorGUIExtension.EndAlpha();
        }

        public static Rect BeginVerticalBoxGroup(params GUILayoutOption[] options)
        {
            return EditorGUILayout.BeginVertical(EditorStylesExtension.RoundedBoxStyle, options);
        }

        public static void EndVerticalBoxGroup()
        {
            EditorGUILayout.EndVertical();
        }

        public static Rect BeginHorizontalBoxGroup(params GUILayoutOption[] options)
        {
            return EditorGUILayout.BeginHorizontal(EditorStylesExtension.RoundedBoxStyle, options);
        }

        public static void EndHorizontalBoxGroup()
        {
            EditorGUILayout.EndHorizontal();
        }

        public static (bool foldout, bool enable) BeginToggleGroup(string label, bool foldout, bool enable)
        {
            BeginVerticalBoxGroup();
            Rect rect = GUILayoutUtility.GetRect(50, 25);
            rect = EditorGUI.IndentedRect(rect);
            Rect toggleRect = new Rect(rect.x + 20, rect.y, rect.height, rect.height);

            Event current = Event.current;
            if (current.type == EventType.MouseDown && current.button == 0)
            {
                if (toggleRect.Contains(current.mousePosition))
                {
                    enable = !enable;
                }
                else if (rect.Contains(current.mousePosition))
                {
                    foldout = !foldout;
                }
            }

            switch (current.type)
            {
                case EventType.MouseDown:
                case EventType.MouseUp:
                case EventType.Repaint:
                    GUI.Box(rect, string.Empty, GUI.skin.button);

                    Rect t = rect;
                    t.xMin += 5;
                    t.xMax = t.xMin + t.height;
                    EditorGUI.Foldout(t, foldout, string.Empty);

                    toggleRect.width = rect.width - t.width;
                    EditorGUI.ToggleLeft(toggleRect, label, enable);
                    break;
                default:
                    break;
            }

            EditorGUI.BeginDisabledGroup(!enable);
            EditorGUI.indentLevel++;
            return (foldout, enable);
        }

        public static void EndToggleGroup()
        {
            EditorGUI.indentLevel--;
            EditorGUI.EndDisabledGroup();
            EndVerticalBoxGroup();
        }

        public static bool Foldout(string label, bool foldout)
        {
            Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(25));
            rect = EditorGUI.IndentedRect(rect);
            return EditorGUIExtension.FoldoutBar(rect, label, foldout);
        }

        public static bool BeginFoldoutGroup(string label, bool foldout)
        {
            BeginVerticalBoxGroup();
            foldout = Foldout(label, foldout);
            EditorGUI.indentLevel++;
            return foldout;
        }

        public static void EndFoldoutGroup()
        {
            EditorGUI.indentLevel--;
            EndVerticalBoxGroup();
        }

        public static float ScrollList(SerializedProperty list, float scroll, ref bool foldout, int count = 10)
        {
            foldout = EditorGUILayout.Foldout(foldout, list.displayName, true);

            if (foldout)
            {
                EditorGUI.indentLevel++;

                GUILayout.BeginHorizontal();
                int size = EditorGUILayout.DelayedIntField("Count", list.arraySize);
                EditorGUI.indentLevel--;
                int targetIndex = -1;
                targetIndex = EditorGUILayout.DelayedIntField(targetIndex, GUILayout.Width(40));
                GUILayout.EndHorizontal();

                EditorGUI.indentLevel++;

                if (size != list.arraySize)
                    list.arraySize = size;

                GUILayout.BeginHorizontal();
                Rect r = EditorGUILayout.BeginVertical();

                if (list.arraySize > count)
                {
                    int startIndex = Mathf.CeilToInt(list.arraySize * scroll);
                    startIndex = Mathf.Max(0, startIndex);
                    for (int i = startIndex; i < startIndex + count; i++)
                    {
                        EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
                    }
                }
                else
                {
                    for (int i = 0; i < list.arraySize; i++)
                    {
                        EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
                    }
                }

                EditorGUILayout.EndVertical();
                if (list.arraySize > count)
                {
                    GUILayout.Space(20);
                    if (list.arraySize > count)
                    {
                        if (Event.current.type == EventType.ScrollWheel && r.Contains(Event.current.mousePosition))
                        {
                            scroll += Event.current.delta.y * 0.01f;
                            Event.current.Use();
                        }
                        if (targetIndex != -1)
                        {
                            scroll = Mathf.Clamp01((float)targetIndex / list.arraySize);
                        }

                        r.xMin += r.width + 5;
                        r.width = 20;
                        scroll = GUI.VerticalScrollbar(r, scroll, (float)count / list.arraySize, 0, 1);
                    }
                }
                GUILayout.EndHorizontal();
                EditorGUI.indentLevel--;

            }
            return scroll;
        }

        public static string FilePath(string label, string path)
        {
            EditorGUILayout.BeginHorizontal();
            path = EditorGUILayout.TextField(label, path);
            Rect rect = GUILayoutUtility.GetLastRect();
            UnityObject uObj = EditorGUIExtension.DragDropAreaSingle(rect, DragAndDropVisualMode.Copy);
            if (uObj != null && AssetDatabase.IsMainAsset(uObj))
            {
                string p = AssetDatabase.GetAssetPath(uObj);
                if (!AssetDatabase.IsValidFolder(p))
                    path = p;
            }
            if (GUILayout.Button(EditorGUIUtility.FindTexture("FolderEmpty Icon"), EditorStylesExtension.OnlyIconButtonStyle, GUILayout.Width(18), GUILayout.Height(18)))
            {
                path = EditorUtility.OpenFilePanel("Select File", Application.dataPath, "*");
                if (!string.IsNullOrEmpty(path))
                    path = path.Substring(path.IndexOf("Assets"));
            }
            EditorGUILayout.EndHorizontal();
            return path;
        }

        public static void FilePath(string label, SerializedProperty pathProperty)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(pathProperty);
            Rect rect = GUILayoutUtility.GetLastRect();
            UnityObject uObj = EditorGUIExtension.DragDropAreaSingle(rect, DragAndDropVisualMode.Copy);
            if (uObj != null && AssetDatabase.IsMainAsset(uObj))
            {
                string p = AssetDatabase.GetAssetPath(uObj);
                if (!AssetDatabase.IsValidFolder(p))
                    pathProperty.stringValue = p;
            }
            if (GUILayout.Button(EditorGUIUtility.FindTexture("FolderEmpty Icon"), EditorStylesExtension.OnlyIconButtonStyle, GUILayout.Width(18), GUILayout.Height(18)))
            {
                string p = EditorUtility.OpenFilePanel("Select File", Application.dataPath, "*");
                if (!string.IsNullOrEmpty(p))
                    pathProperty.stringValue = p.Substring(p.IndexOf("Assets"));
            }
            EditorGUILayout.EndHorizontal();
        }

        public static string FolderPath(string label, string folder)
        {
            EditorGUILayout.BeginHorizontal();
            folder = EditorGUILayout.TextField(label, folder);
            Rect rect = GUILayoutUtility.GetLastRect();
            UnityObject uObj = EditorGUIExtension.DragDropAreaSingle(rect, DragAndDropVisualMode.Copy);
            if (uObj != null && AssetDatabase.IsMainAsset(uObj))
            {
                string p = AssetDatabase.GetAssetPath(uObj);
                if (AssetDatabase.IsValidFolder(p))
                    folder = p;
            }
            if (GUILayout.Button(EditorGUIUtility.FindTexture("FolderEmpty Icon"), EditorStylesExtension.OnlyIconButtonStyle, GUILayout.Width(18), GUILayout.Height(18)))
            {
                folder = EditorUtility.OpenFolderPanel("Select Folder", Application.dataPath, string.Empty);
                if (!string.IsNullOrEmpty(folder))
                    folder = folder.Substring(folder.IndexOf("Assets"));
            }
            EditorGUILayout.EndHorizontal();
            return folder;
        }

        public static void FolderPath(string label, SerializedProperty folder)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(folder);
            Rect rect = GUILayoutUtility.GetLastRect();
            UnityObject uObj = EditorGUIExtension.DragDropAreaSingle(rect, DragAndDropVisualMode.Copy);
            if (uObj != null && AssetDatabase.IsMainAsset(uObj))
            {
                string p = AssetDatabase.GetAssetPath(uObj);
                if (AssetDatabase.IsValidFolder(p))
                    folder.stringValue = p;
            }
            if (GUILayout.Button(EditorGUIUtility.FindTexture("FolderEmpty Icon"), EditorStylesExtension.OnlyIconButtonStyle, GUILayout.Width(18), GUILayout.Height(18)))
            {
                string p = EditorUtility.OpenFolderPanel("Select Folder", Application.dataPath, string.Empty);
                if (!string.IsNullOrEmpty(p))
                    folder.stringValue = p.Substring(p.IndexOf("Assets"));
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif