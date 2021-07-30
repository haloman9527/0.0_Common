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
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace CZToolKit.Core.Editors
{
    public static partial class EditorGUILayoutExtension
    {
        public static Rect BeginBox()
        {
            return EditorGUILayout.BeginVertical(GUI.skin.box);
        }

        public static void EndBox()
        {
            EditorGUILayout.EndVertical();
        }

        public static float ScrollList(SerializedProperty _list, float _scroll, ref bool _foldout, int _count = 10)
        {
            _foldout = EditorGUILayout.Foldout(_foldout, _list.displayName, true);

            if (_foldout)
            {
                EditorGUI.indentLevel++;

                GUILayout.BeginHorizontal();
                int size = EditorGUILayout.DelayedIntField("Count", _list.arraySize);
                EditorGUI.indentLevel--;
                int targetIndex = -1;
                targetIndex = EditorGUILayout.DelayedIntField(targetIndex, GUILayout.Width(40));
                GUILayout.EndHorizontal();

                EditorGUI.indentLevel++;

                if (size != _list.arraySize)
                    _list.arraySize = size;

                GUILayout.BeginHorizontal();
                Rect r = EditorGUILayout.BeginVertical();

                if (_list.arraySize > _count)
                {
                    int startIndex = Mathf.CeilToInt(_list.arraySize * _scroll);
                    startIndex = Mathf.Max(0, startIndex);
                    for (int i = startIndex; i < startIndex + _count; i++)
                    {
                        EditorGUILayout.PropertyField(_list.GetArrayElementAtIndex(i));
                    }
                }
                else
                {
                    for (int i = 0; i < _list.arraySize; i++)
                    {
                        EditorGUILayout.PropertyField(_list.GetArrayElementAtIndex(i));
                    }
                }

                EditorGUILayout.EndVertical();
                GUILayout.Space(20);
                GUILayout.EndHorizontal();
                EditorGUI.indentLevel--;

                if (Event.current.type == EventType.ScrollWheel && r.Contains(Event.current.mousePosition))
                {
                    _scroll += Event.current.delta.y * 0.01f;
                    Event.current.Use();
                }
                if (targetIndex != -1)
                {
                    _scroll = Mathf.Clamp01((float)targetIndex / _list.arraySize);
                }

                r.xMin += r.width + 5;
                r.width = 20;
                _scroll = GUI.VerticalScrollbar(r, _scroll, (float)_count / _list.arraySize, 0, 1);
            }
            return _scroll;
        }

        public static bool BeginToggleGroup(string _label, bool _foldout, ref bool _enable)
        {
            Rect rect = GUILayoutUtility.GetRect(50, 25);
            rect = EditorGUI.IndentedRect(rect);
            rect.xMin += 3f;
            rect.xMax -= 3f;

            Rect toggleRect = new Rect(rect.x + 10, rect.y, rect.height, rect.height);
            if (Event.current.type == EventType.Repaint)
            {
                GUI.Box(rect, string.Empty, GUI.skin.button);

                Rect t = rect;
                t.xMin = t.xMax - t.height;
                EditorGUI.Foldout(t, _foldout, string.Empty);

                EditorGUI.ToggleLeft(toggleRect, _label, _enable);
            }

            Event current = Event.current;
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                if (toggleRect.Contains(current.mousePosition))
                {
                    _enable = !_enable;
                    current.Use();
                }
                else if (rect.Contains(current.mousePosition))
                {
                    _foldout = !_foldout;
                    current.Use();
                }
            }

            EditorGUI.BeginDisabledGroup(!_enable);
            EditorGUI.indentLevel++;
            return _foldout;
        }

        public static void EndToggleGroup()
        {
            EditorGUI.indentLevel--;
            EditorGUI.EndDisabledGroup();
        }
    }
}
