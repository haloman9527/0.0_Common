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
using UnityEngine;
using UnityEditor;

namespace Atom.UnityEditors.IMGUI.Controls
{
    [Serializable]
    public class SplitView
    {
        public enum Direction
        {
            Horizontal,
            Vertical
        }

        private Direction splitDirection;
        private float splitNormalizedPosition;
        private bool resize;
        public Rect availableRect;
        public Vector2 scrollPosition;


        public SplitView(Direction splitDirection)
        {
            splitNormalizedPosition = 0.5f;
            this.splitDirection = splitDirection;
        }

        public void BeginSplitView()
        {
            Rect tempRect;

            if (splitDirection == Direction.Horizontal)
                tempRect = EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            else
                tempRect = EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));

            if (tempRect.width > 0.0f)
            {
                tempRect.width = Mathf.Max(tempRect.width, 10);
                availableRect = tempRect;
            }

            if (splitDirection == Direction.Horizontal)
                scrollPosition = GUILayout.BeginScrollView(scrollPosition,
                    GUILayout.Width(availableRect.width * splitNormalizedPosition));
            else
                scrollPosition = GUILayout.BeginScrollView(scrollPosition,
                    GUILayout.Height(availableRect.height * splitNormalizedPosition));
        }

        public void Split()
        {
            GUILayout.EndScrollView();
            GUILayout.Space(5);
            ResizeSplitFirstView();
        }

        public void EndSplitView()
        {
            if (splitDirection == Direction.Horizontal)
                EditorGUILayout.EndHorizontal();
            else
                EditorGUILayout.EndVertical();
        }

        private void ResizeSplitFirstView()
        {
            Rect resizeHandleRect;

            if (splitDirection == Direction.Horizontal)
                resizeHandleRect = new Rect(availableRect.width * splitNormalizedPosition, availableRect.y, 5f,
                    availableRect.height);
            else
                resizeHandleRect = new Rect(availableRect.x, availableRect.height * splitNormalizedPosition,
                    availableRect.width, 5f);

            var resizeHandleRect2 = resizeHandleRect;
            resizeHandleRect2.width = 2;
            resizeHandleRect2.x += 1;
            GUI.DrawTexture(resizeHandleRect2, EditorGUIUtility.whiteTexture);

            if (splitDirection == Direction.Horizontal)
                EditorGUIUtility.AddCursorRect(resizeHandleRect, MouseCursor.ResizeHorizontal);
            else
                EditorGUIUtility.AddCursorRect(resizeHandleRect, MouseCursor.ResizeVertical);

            if (Event.current.type == EventType.MouseDown && resizeHandleRect.Contains(Event.current.mousePosition))
            {
                resize = true;
            }

            if (resize)
            {
                if (splitDirection == Direction.Horizontal)
                    splitNormalizedPosition = Event.current.mousePosition.x / availableRect.width;
                else
                    splitNormalizedPosition = Event.current.mousePosition.y / availableRect.height;
            }

            if (Event.current.type == EventType.MouseUp)
                resize = false;
        }
    }
}
#endif