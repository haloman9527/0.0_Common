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
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace CZToolKit.Core.Editors
{
    [Serializable]
    public class ResizableArea
    {
        public enum UIDirection
        {
            None = 0,
            Left = 1 << 0,
            Right = 1 << 1,
            Top = 1 << 2,
            Bottom = 1 << 3,
            TopLeft = 1 << 4,
            TopRight = 1 << 5,
            MiddleCenter = 1 << 6,
            BottomLeft = 1 << 7,
            BottomRight = 1 << 8
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

        public void SetEnable(UIDirection direction, bool enable)
        {
            if (enable)
                enabledSides |= direction;
            else
            {
                enabledSides &= ~direction;
                if (Sides.ContainsKey(direction))
                    Sides.Remove(direction);
            }
        }

        public bool IsEnabled(UIDirection direction)
        {
            return enabledSides.HasFlag(direction);
        }

        void Reload(Rect _rect)
        {
            foreach (var direction in Directions)
            {
                if (enabledSides.HasFlag(direction))
                {
                    float offset;
                    SideOffset.TryGetValue(direction, out offset);
                    Sides[direction] = GetSide(_rect, direction, side, offset);
                }
            }
        }

        public Rect GetSide(Rect self, UIDirection sideDirection, float sideWidth, float offset = 0)
        {
            switch (sideDirection)
            {
                case UIDirection.MiddleCenter:
                    return new Rect(self.x + sideWidth / 2, self.y + sideWidth / 2, self.width - sideWidth, self.height - sideWidth);
                case UIDirection.Top:
                    return new Rect(self.x + sideWidth / 2, self.y - sideWidth / 2 + offset, self.width - sideWidth, sideWidth);
                case UIDirection.Bottom:
                    return new Rect(self.x + sideWidth / 2, self.y + self.height - sideWidth / 2 + offset, self.width - sideWidth, sideWidth);
                case UIDirection.Left:
                    return new Rect(self.x - sideWidth / 2 + offset, self.y + sideWidth / 2, sideWidth, self.height - sideWidth);
                case UIDirection.Right:
                    return new Rect(self.x + self.width - sideWidth / 2 + offset, self.y + sideWidth / 2, sideWidth, self.height - sideWidth);
                case UIDirection.TopLeft:
                    return new Rect(self.x - sideWidth / 2 + offset, self.y - sideWidth / 2 + offset, sideWidth, sideWidth);
                case UIDirection.TopRight:
                    return new Rect(self.x + self.width - sideWidth / 2 + offset, self.y - sideWidth / 2 + offset, sideWidth, sideWidth);
                case UIDirection.BottomLeft:
                    return new Rect(self.x - sideWidth / 2 + offset, self.y + self.height - sideWidth / 2 + offset, sideWidth, sideWidth);
                case UIDirection.BottomRight:
                    return new Rect(self.x + self.width - sideWidth / 2 + offset, self.y + self.height - sideWidth / 2 + offset, sideWidth, sideWidth);
            }
            return new Rect();
        }

        public virtual Rect OnGUI(Rect position)
        {
            Reload(position);
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
                                    position.yMin = evt.mousePosition.y;
                                }
                                break;
                            case UIDirection.Bottom:
                                if (IsEnabled(sideDirection))
                                {
                                    position.yMax = evt.mousePosition.y;
                                }
                                break;
                            case UIDirection.Left:
                                if (IsEnabled(sideDirection))
                                {
                                    position.xMin += evt.mousePosition.x;
                                }
                                break;
                            case UIDirection.Right:
                                if (IsEnabled(sideDirection))
                                {
                                    position.xMax = evt.mousePosition.x;
                                }
                                break;
                            case UIDirection.TopLeft:
                                if (IsEnabled(sideDirection))
                                {
                                    position.yMin = evt.mousePosition.y;

                                    position.xMin += evt.mousePosition.x;
                                }
                                break;
                            case UIDirection.TopRight:
                                if (IsEnabled(sideDirection))
                                {
                                    position.yMin = evt.mousePosition.y;

                                    position.xMax = evt.mousePosition.x;
                                }
                                break;
                            case UIDirection.BottomLeft:
                                if (IsEnabled(sideDirection))
                                {
                                    position.yMax = evt.mousePosition.y;

                                    position.xMin = evt.mousePosition.x;
                                }
                                break;
                            case UIDirection.BottomRight:
                                if (IsEnabled(sideDirection))
                                {
                                    position.yMax = evt.mousePosition.y;

                                    position.xMax = evt.mousePosition.x;
                                }
                                break;
                            case UIDirection.MiddleCenter:
                                if (IsEnabled(sideDirection))
                                    position.position += evt.delta;
                                break;

                        }
                        evt.Use();
                    }
                    break;
                default:
                    break;
            }

            position.width = Mathf.Max(position.width, minSize.x);
            position.height = Mathf.Max(position.height, minSize.y);

            if (maxSize != Vector2.zero)
            {
                position.width = Mathf.Min(position.width, maxSize.x);
                position.height = Mathf.Min(position.height, maxSize.y);
            }

            return position;
        }
    }
}
#endif