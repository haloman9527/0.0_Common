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
