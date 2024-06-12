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

namespace CZToolKitEditor.IMGUI.Controls
{
    [Serializable]
    public class ResizableArea
    {
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

        public const float DefaultSide = 10;

        bool isDragging;

        UIDirection[] directions;
        UIDirection sideDirection;
        UIDirection enabledSides;
        Dictionary<UIDirection, Rect> sides;
        Dictionary<UIDirection, float> sideOffset;

        public Vector2 minSize = Vector2.zero;
        public Vector2 maxSize = Vector2.zero;
        public float side = DefaultSide;

        Dictionary<UIDirection, Rect> Sides
        {
            get
            {
                if (sides == null)
                    sides = new Dictionary<UIDirection, Rect>();
                return sides;
            }
        }

        public Dictionary<UIDirection, float> SideOffset
        {
            get
            {
                if (sideOffset == null)
                    sideOffset = new Dictionary<UIDirection, float>();
                return sideOffset;
            }
        }

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
            if (Sides.ContainsKey(direction))
                Sides.Remove(direction);
        }

        public bool IsEnabled(UIDirection direction)
        {
            return enabledSides.HasFlag(direction);
        }

        void Reload(Rect rect)
        {
            foreach (var direction in Directions)
            {
                if (enabledSides.HasFlag(direction))
                {
                    float offset;
                    SideOffset.TryGetValue(direction, out offset);
                    Sides[direction] = GetSide(rect, direction, side, offset);
                }
            }
        }

        public Rect GetSide(Rect self, UIDirection direction, float width, float offset = 0)
        {
            switch (direction)
            {
                case UIDirection.MiddleCenter:
                    return new Rect(self.x + width / 2, self.y + width / 2, self.width - width, self.height - width);
                case UIDirection.Top:
                    return new Rect(self.x + width / 2, self.y - width / 2 + offset, self.width - width, width);
                case UIDirection.Bottom:
                    return new Rect(self.x + width / 2, self.y + self.height - width / 2 + offset, self.width - width, width);
                case UIDirection.Left:
                    return new Rect(self.x - width / 2 + offset, self.y + width / 2, width, self.height - width);
                case UIDirection.Right:
                    return new Rect(self.x + self.width - width / 2 + offset, self.y + width / 2, width, self.height - width);
                case UIDirection.TopLeft:
                    return new Rect(self.x - width / 2 + offset, self.y - width / 2 + offset, width, width);
                case UIDirection.TopRight:
                    return new Rect(self.x + self.width - width / 2 + offset, self.y - width / 2 + offset, width, width);
                case UIDirection.BottomLeft:
                    return new Rect(self.x - width / 2 + offset, self.y + self.height - width / 2 + offset, width, width);
                case UIDirection.BottomRight:
                    return new Rect(self.x + self.width - width / 2 + offset, self.y + self.height - width / 2 + offset, width, width);
            }

            return new Rect();
        }

        public virtual Rect OnGUI(Rect rect)
        {
            Reload(rect);
            Event evt = Event.current;
            switch (evt.type)
            {
                case EventType.Repaint:
                    if (IsEnabled(UIDirection.Top))
                        EditorGUIUtility.AddCursorRect(Sides[UIDirection.Top], MouseCursor.ResizeVertical);
                    if (IsEnabled(UIDirection.Bottom))
                        EditorGUIUtility.AddCursorRect(Sides[UIDirection.Bottom], MouseCursor.ResizeVertical);

                    if (IsEnabled(UIDirection.Left))
                        EditorGUIUtility.AddCursorRect(Sides[UIDirection.Left], MouseCursor.ResizeHorizontal);
                    if (IsEnabled(UIDirection.Right))
                        EditorGUIUtility.AddCursorRect(Sides[UIDirection.Right], MouseCursor.ResizeHorizontal);

                    if (IsEnabled(UIDirection.TopLeft))
                        EditorGUIUtility.AddCursorRect(Sides[UIDirection.TopLeft], MouseCursor.ResizeUpLeft);
                    if (IsEnabled(UIDirection.TopRight))
                        EditorGUIUtility.AddCursorRect(Sides[UIDirection.TopRight], MouseCursor.ResizeUpRight);

                    if (IsEnabled(UIDirection.BottomLeft))
                        EditorGUIUtility.AddCursorRect(Sides[UIDirection.BottomLeft], MouseCursor.ResizeUpRight);
                    if (IsEnabled(UIDirection.BottomRight))
                        EditorGUIUtility.AddCursorRect(Sides[UIDirection.BottomRight], MouseCursor.ResizeUpLeft);

                    if (IsEnabled(UIDirection.MiddleCenter) && isDragging && sideDirection == UIDirection.MiddleCenter)
                        EditorGUIUtility.AddCursorRect(Sides[UIDirection.MiddleCenter], MouseCursor.MoveArrow);
                    break;
                case EventType.MouseDown:
                    foreach (var direction in Directions)
                    {
                        if (IsEnabled(direction) && Sides[direction].Contains(evt.mousePosition))
                        {
                            sideDirection = direction;
                            isDragging = true;
                            Event.current.Use();
                            break;
                        }
                    }

                    break;
                case EventType.MouseUp:
                    isDragging = false;
                    sideDirection = UIDirection.None;
                    break;
                case EventType.MouseDrag:
                    if (isDragging)
                    {
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
                    }

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

            return rect;
        }
    }
}
#endif