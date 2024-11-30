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
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace MoyoEditor.IMGUI.Controls
{
    [Serializable]
    public class ResizableArea
    {
        [Flags]
        public enum UIDirection
        {
            None = 0,
            Left = 1,
            Right = 2,
            Top = 4,
            Bottom = 8,
            TopLeft = 16,
            TopRight = 32,
            MiddleCenter = 64,
            BottomLeft = 128,
            BottomRight = 256
        }

        public const float DefaultSideWdith = 10;

        public static readonly Dictionary<UIDirection, int> DirectionIndexMap = new Dictionary<UIDirection, int>
        {
            { UIDirection.None, -1 },
            { UIDirection.Left, 0 },
            { UIDirection.Right, 1 },
            { UIDirection.Top, 1 },
            { UIDirection.Bottom, 1 },
            { UIDirection.TopLeft, 1 },
            { UIDirection.TopRight, 1 },
            { UIDirection.MiddleCenter, 1 },
            { UIDirection.BottomLeft, 1 },
            { UIDirection.BottomRight, 1 },
        };

        [NonSerialized] private bool isDragging;
        [NonSerialized] private UIDirection[] directions;

        public UIDirection sideDirection;
        public UIDirection enabledSides;
        public Rect[] sides = new Rect[9];
        public float[] sideOffset = new float[9];

        public Rect rect = default;
        public Vector2 minSize = Vector2.zero;
        public Vector2 maxSize = Vector2.zero;
        public float side = DefaultSideWdith;

        UIDirection[] Directions
        {
            get
            {
                if (directions == null)
                {
                    Array array = Enum.GetValues(typeof(UIDirection));
                    directions = new UIDirection[array.Length];
                    for (int i = 0; i < directions.Length; i++)
                    {
                        UIDirection direction = (UIDirection)(array.GetValue(i));
                        directions[i] = direction;
                    }
                }

                return directions;
            }
        }

        public ResizableArea()
        {
            directions = Directions;
        }

        public void EnableSide(UIDirection direction)
        {
            enabledSides |= direction;
        }

        public void DisableSide(UIDirection direction)
        {
            enabledSides &= ~direction;
        }

        public bool IsEnabled(UIDirection direction)
        {
            return (enabledSides & direction) != UIDirection.None;
        }

        void Reload()
        {
            foreach (var direction in Directions)
            {
                if ((enabledSides & direction) == UIDirection.None)
                {
                    continue;
                }

                sides[DirectionIndexMap[direction]] = GetSide(direction, side, sideOffset[DirectionIndexMap[direction]]);
            }
        }

        public Rect GetSide(UIDirection direction, float width, float offset = 0)
        {
            switch (direction)
            {
                case UIDirection.MiddleCenter:
                    return new Rect(rect.x + width / 2, rect.y + width / 2, rect.width - width, rect.height - width);
                case UIDirection.Top:
                    return new Rect(rect.x + width / 2, rect.y - width / 2 + offset, rect.width - width, width);
                case UIDirection.Bottom:
                    return new Rect(rect.x + width / 2, rect.y + rect.height - width / 2 + offset, rect.width - width, width);
                case UIDirection.Left:
                    return new Rect(rect.x - width / 2 + offset, rect.y + width / 2, width, rect.height - width);
                case UIDirection.Right:
                    return new Rect(rect.x + rect.width - width / 2 + offset, rect.y + width / 2, width, rect.height - width);
                case UIDirection.TopLeft:
                    return new Rect(rect.x - width / 2 + offset, rect.y - width / 2 + offset, width, width);
                case UIDirection.TopRight:
                    return new Rect(rect.x + rect.width - width / 2 + offset, rect.y - width / 2 + offset, width, width);
                case UIDirection.BottomLeft:
                    return new Rect(rect.x - width / 2 + offset, rect.y + rect.height - width / 2 + offset, width, width);
                case UIDirection.BottomRight:
                    return new Rect(rect.x + rect.width - width / 2 + offset, rect.y + rect.height - width / 2 + offset, width, width);
            }

            return new Rect();
        }

        public virtual void OnGUI()
        {
            Reload();
            Event evt = Event.current;
            switch (evt.type)
            {
                case EventType.Repaint:
                    if (IsEnabled(UIDirection.Top))
                        EditorGUIUtility.AddCursorRect(sides[DirectionIndexMap[UIDirection.Top]], MouseCursor.ResizeVertical);
                    if (IsEnabled(UIDirection.Bottom))
                        EditorGUIUtility.AddCursorRect(sides[DirectionIndexMap[UIDirection.Bottom]], MouseCursor.ResizeVertical);

                    if (IsEnabled(UIDirection.Left))
                        EditorGUIUtility.AddCursorRect(sides[DirectionIndexMap[UIDirection.Left]], MouseCursor.ResizeHorizontal);
                    if (IsEnabled(UIDirection.Right))
                        EditorGUIUtility.AddCursorRect(sides[DirectionIndexMap[UIDirection.Right]], MouseCursor.ResizeHorizontal);

                    if (IsEnabled(UIDirection.TopLeft))
                        EditorGUIUtility.AddCursorRect(sides[DirectionIndexMap[UIDirection.TopLeft]], MouseCursor.ResizeUpLeft);
                    if (IsEnabled(UIDirection.TopRight))
                        EditorGUIUtility.AddCursorRect(sides[DirectionIndexMap[UIDirection.TopRight]], MouseCursor.ResizeUpRight);

                    if (IsEnabled(UIDirection.BottomLeft))
                        EditorGUIUtility.AddCursorRect(sides[DirectionIndexMap[UIDirection.BottomLeft]], MouseCursor.ResizeUpRight);
                    if (IsEnabled(UIDirection.BottomRight))
                        EditorGUIUtility.AddCursorRect(sides[DirectionIndexMap[UIDirection.BottomRight]], MouseCursor.ResizeUpLeft);

                    if (IsEnabled(UIDirection.MiddleCenter) && isDragging && sideDirection == UIDirection.MiddleCenter)
                        EditorGUIUtility.AddCursorRect(sides[DirectionIndexMap[UIDirection.MiddleCenter]], MouseCursor.MoveArrow);
                    break;
                case EventType.MouseDown:
                    foreach (var direction in Directions)
                    {
                        if (!IsEnabled(direction) || !sides[DirectionIndexMap[direction]].Contains(evt.mousePosition))
                        {
                            continue;
                        }

                        sideDirection = direction;
                        isDragging = true;
                        Event.current.Use();
                        break;
                    }

                    break;
                case EventType.MouseUp:
                    isDragging = false;
                    sideDirection = UIDirection.None;
                    break;
                case EventType.MouseDrag:
                    if (!isDragging)
                    {
                        break;
                    }

                    switch (sideDirection)
                    {
                        case UIDirection.Top:
                            if (IsEnabled(sideDirection))
                            {
                                rect.yMin = evt.mousePosition.y;
                            }

                            break;
                        case UIDirection.Bottom:
                            if (IsEnabled(sideDirection))
                            {
                                rect.yMax = evt.mousePosition.y;
                            }

                            break;
                        case UIDirection.Left:
                            if (IsEnabled(sideDirection))
                            {
                                rect.xMin += evt.mousePosition.x;
                            }

                            break;
                        case UIDirection.Right:
                            if (IsEnabled(sideDirection))
                            {
                                rect.xMax = evt.mousePosition.x;
                            }

                            break;
                        case UIDirection.TopLeft:
                            if (IsEnabled(sideDirection))
                            {
                                rect.yMin = evt.mousePosition.y;

                                rect.xMin += evt.mousePosition.x;
                            }

                            break;
                        case UIDirection.TopRight:
                            if (IsEnabled(sideDirection))
                            {
                                rect.yMin = evt.mousePosition.y;

                                rect.xMax = evt.mousePosition.x;
                            }

                            break;
                        case UIDirection.BottomLeft:
                            if (IsEnabled(sideDirection))
                            {
                                rect.yMax = evt.mousePosition.y;

                                rect.xMin = evt.mousePosition.x;
                            }

                            break;
                        case UIDirection.BottomRight:
                            if (IsEnabled(sideDirection))
                            {
                                rect.yMax = evt.mousePosition.y;

                                rect.xMax = evt.mousePosition.x;
                            }

                            break;
                        case UIDirection.MiddleCenter:
                            if (IsEnabled(sideDirection))
                                rect.position += evt.delta;
                            break;
                    }

                    evt.Use();

                    break;
                default:
                    break;
            }

            rect.width = Mathf.Max(rect.width, minSize.x);
            rect.height = Mathf.Max(rect.height, minSize.y);

            if (maxSize != Vector2.zero)
            {
                rect.width = Mathf.Min(rect.width, maxSize.x);
                rect.height = Mathf.Min(rect.height, maxSize.y);
            }
        }
    }
}
#endif