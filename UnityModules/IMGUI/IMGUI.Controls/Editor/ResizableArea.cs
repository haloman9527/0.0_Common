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

namespace CZToolKit.Common.IMGUI.Controls
{
    [Serializable]
    public class ResizableArea
    {
        [Flags]
        public enum Direction
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

        public const float DefaultSideWidth = 10;

        private Direction activeSides;
        private Direction draggingSide;
        private float sideWidth = DefaultSideWidth;

        public Vector2 minSize = Vector2.zero;
        public Vector2 maxSize = Vector2.zero;

        private Rect GetSide(Rect rect, Direction direction, float offset = 0)
        {
            switch (direction)
            {
                case Direction.MiddleCenter:
                    return new Rect(rect.x + sideWidth / 2, rect.y + sideWidth / 2, rect.width - sideWidth,
                        rect.height - sideWidth);
                case Direction.Top:
                    return new Rect(rect.x + sideWidth / 2, rect.y - sideWidth / 2 + offset, rect.width - sideWidth,
                        sideWidth);
                case Direction.Bottom:
                    return new Rect(rect.x + sideWidth / 2, rect.y + rect.height - sideWidth / 2 + offset,
                        rect.width - sideWidth, sideWidth);
                case Direction.Left:
                    return new Rect(rect.x - sideWidth / 2 + offset, rect.y + sideWidth / 2, sideWidth,
                        rect.height - sideWidth);
                case Direction.Right:
                    return new Rect(rect.x + rect.width - sideWidth / 2 + offset, rect.y + sideWidth / 2, sideWidth,
                        rect.height - sideWidth);
                case Direction.TopLeft:
                    return new Rect(rect.x - sideWidth / 2 + offset, rect.y - sideWidth / 2 + offset, sideWidth,
                        sideWidth);
                case Direction.TopRight:
                    return new Rect(rect.x + rect.width - sideWidth / 2 + offset, rect.y - sideWidth / 2 + offset,
                        sideWidth, sideWidth);
                case Direction.BottomLeft:
                    return new Rect(rect.x - sideWidth / 2 + offset, rect.y + rect.height - sideWidth / 2 + offset,
                        sideWidth, sideWidth);
                case Direction.BottomRight:
                    return new Rect(rect.x + rect.width - sideWidth / 2 + offset,
                        rect.y + rect.height - sideWidth / 2 + offset, sideWidth, sideWidth);
            }

            return new Rect();
        }

        public void EnableSide(Direction direction)
        {
            activeSides |= direction;
        }

        public void DisableSide(Direction direction)
        {
            activeSides &= ~direction;
        }

        public bool IsEnabled(Direction direction)
        {
            return (activeSides & direction) == direction;
        }
        
        public Rect OnGUI(Rect position)
        {
            Event evt = Event.current;
            switch (evt.type)
            {
                case EventType.Repaint:
                {
                    for (int i = 0; i <= 9; i++)
                    {
                        var direction = (Direction)(1 << i);
                        if (!IsEnabled(direction))
                            continue;

                        var side = GetSide(position, direction, 10);
                        var mouseCursor = MouseCursor.MoveArrow;
                        switch (direction)
                        {
                            case Direction.Top:
                            case Direction.Bottom:
                                mouseCursor = MouseCursor.ResizeVertical;
                                break;
                            case Direction.Left:
                            case Direction.Right:
                                mouseCursor = MouseCursor.ResizeHorizontal;
                                break;
                            case Direction.TopLeft:
                                mouseCursor = MouseCursor.ResizeUpLeft;
                                break;
                            case Direction.TopRight:
                                mouseCursor = MouseCursor.ResizeUpRight;
                                break;
                            case Direction.BottomLeft:
                                mouseCursor = MouseCursor.ResizeUpRight;
                                break;
                            case Direction.BottomRight:
                                mouseCursor = MouseCursor.ResizeUpLeft;
                                break;
                            case Direction.MiddleCenter:
                                if (draggingSide != Direction.MiddleCenter)
                                    continue;
                                mouseCursor = MouseCursor.MoveArrow;
                                break;
                        }

                        EditorGUIUtility.AddCursorRect(side, mouseCursor);
                    }

                    break;
                }
                case EventType.MouseDown:
                {
                    for (int i = 0; i <= 9; i++)
                    {
                        var direction = (Direction)(1 << i);
                        if (!IsEnabled(direction))
                            continue;

                        var side = GetSide(position, direction, 10);
                        if (IsEnabled(direction) && side.Contains(evt.mousePosition))
                        {
                            draggingSide = direction;
                            Event.current.Use();
                            break;
                        }
                    }

                    break;
                }
                case EventType.MouseUp:
                {
                    draggingSide = Direction.None;
                    break;
                }
                case EventType.MouseDrag:
                {
                    if (draggingSide == Direction.None)
                        break;
                    switch (draggingSide)
                    {
                        case Direction.Top:
                            if (IsEnabled(draggingSide))
                                position.yMin = evt.mousePosition.y;

                            break;
                        case Direction.Bottom:
                            if (IsEnabled(draggingSide))
                                position.yMax = evt.mousePosition.y;

                            break;
                        case Direction.Left:
                            if (IsEnabled(draggingSide))
                                position.xMin += evt.mousePosition.x;

                            break;
                        case Direction.Right:
                            if (IsEnabled(draggingSide))
                                position.xMax = evt.mousePosition.x;

                            break;
                        case Direction.TopLeft:
                            if (IsEnabled(draggingSide))
                            {
                                position.yMin = evt.mousePosition.y;
                                position.xMin += evt.mousePosition.x;
                            }

                            break;
                        case Direction.TopRight:
                            if (IsEnabled(draggingSide))
                            {
                                position.yMin = evt.mousePosition.y;
                                position.xMax = evt.mousePosition.x;
                            }

                            break;
                        case Direction.BottomLeft:
                            if (IsEnabled(draggingSide))
                            {
                                position.yMax = evt.mousePosition.y;
                                position.xMin = evt.mousePosition.x;
                            }

                            break;
                        case Direction.BottomRight:
                            if (IsEnabled(draggingSide))
                            {
                                position.yMax = evt.mousePosition.y;
                                position.xMax = evt.mousePosition.x;
                            }

                            break;
                        case Direction.MiddleCenter:
                            if (IsEnabled(draggingSide))
                                position.position += evt.delta;
                            
                            break;
                    }

                    evt.Use();
                    break;
                }
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