using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace CZToolKit.Core.Editors
{
    [Serializable]
    public class ResizableArea
    {
        public const float DefaultSide = 10;

        Rect position;
        bool isDragging;

        UIDirections[] directions;
        UIDirections sideDirection;
        UIDirections enabledSides;
        Dictionary<UIDirections, Rect> sides;
        Dictionary<UIDirections, float> sideOffset;

        public Vector2 minSize = Vector2.zero;
        public Vector2 maxSize = Vector2.zero;
        public float side = DefaultSide;

        Dictionary<UIDirections, Rect> Sides
        {
            get
            {
                if (sides == null)
                {
                    sides = new Dictionary<UIDirections, Rect>();
                    Reload();
                }
                return sides;
            }
        }

        public Dictionary<UIDirections, float> SideOffset
        {
            get
            {
                if (sideOffset == null)
                    sideOffset = new Dictionary<UIDirections, float>();
                return sideOffset;
            }
        }
        UIDirections[] Directions
        {
            get
            {
                if (directions == null)
                {
                    Array array = Enum.GetValues(typeof(UIDirections));
                    directions = new UIDirections[array.Length];
                    for (int i = 0; i < directions.Length; i++)
                    {
                        UIDirections direction = (UIDirections)(array.GetValue(i));
                        directions[i] = direction;
                        DisableSide(direction);
                    }
                }
                return directions;
            }
        }
        public Rect Position
        {
            get { return position; }
            set { position = value; }
        }

        public ResizableArea(Rect _position)
        {
            Position = _position;
            Reload();
        }

        public void EnableSide(UIDirections _direction)
        {
            enabledSides |= _direction;
        }


        public void DisableSide(UIDirections _direction)
        {
            enabledSides &= ~_direction;
            if (Sides.ContainsKey(_direction))
                Sides.Remove(_direction);
        }

        void Reload()
        {
            foreach (var direction in Directions)
            {
                if (enabledSides.HasFlag(direction))
                {
                    float offset = 0;
                    SideOffset.TryGetValue(direction, out offset);
                    Sides[direction] = Position.GetSide(direction, side, offset);
                }
            }
        }

        public virtual void OnGUI()
        {
            Reload();
            Event evt = Event.current;
            switch (evt.type)
            {
                case EventType.Repaint:
                    if (enabledSides.HasFlag(UIDirections.Top))
                        EditorGUIUtility.AddCursorRect(Sides[UIDirections.Top], MouseCursor.ResizeVertical);
                    if (enabledSides.HasFlag(UIDirections.Bottom))
                        EditorGUIUtility.AddCursorRect(Sides[UIDirections.Bottom], MouseCursor.ResizeVertical);

                    if (enabledSides.HasFlag(UIDirections.Left))
                        EditorGUIUtility.AddCursorRect(Sides[UIDirections.Left], MouseCursor.ResizeHorizontal);
                    if (enabledSides.HasFlag(UIDirections.Right))
                        EditorGUIUtility.AddCursorRect(Sides[UIDirections.Right], MouseCursor.ResizeHorizontal);

                    if (enabledSides.HasFlag(UIDirections.TopLeft))
                        EditorGUIUtility.AddCursorRect(Sides[UIDirections.TopLeft], MouseCursor.ResizeUpLeft);
                    if (enabledSides.HasFlag(UIDirections.TopRight))
                        EditorGUIUtility.AddCursorRect(Sides[UIDirections.TopRight], MouseCursor.ResizeUpRight);

                    if (enabledSides.HasFlag(UIDirections.BottomLeft))
                        EditorGUIUtility.AddCursorRect(Sides[UIDirections.BottomLeft], MouseCursor.ResizeUpRight);
                    if (enabledSides.HasFlag(UIDirections.BottomRight))
                        EditorGUIUtility.AddCursorRect(Sides[UIDirections.BottomRight], MouseCursor.ResizeUpLeft);

                    if (enabledSides.HasFlag(UIDirections.MiddleCenter) && isDragging && sideDirection == UIDirections.MiddleCenter)
                        EditorGUIUtility.AddCursorRect(Position.GetSide(UIDirections.MiddleCenter, side), MouseCursor.MoveArrow);
                    break;
                case EventType.MouseDown:
                    foreach (var direction in Directions)
                    {
                        if (enabledSides.HasFlag(direction)
                            && Sides[direction].Contains(evt.mousePosition))
                        {
                            sideDirection = direction;
                            isDragging = true;
                            break;
                        }
                    }
                    break;
                case EventType.MouseUp:
                    isDragging = false;
                    sideDirection = UIDirections.None;
                    break;
                case EventType.MouseDrag:
                    if (isDragging)
                    {
                        switch (sideDirection)
                        {
                            case UIDirections.Top:
                                if (enabledSides.HasFlag(sideDirection))
                                {
                                    float deltaY = evt.delta.y;
                                    if (Position.y + deltaY > Position.y + Position.height)
                                        deltaY = 0;
                                    position.y += deltaY;
                                    position.height -= deltaY;
                                }
                                break;
                            case UIDirections.Bottom:
                                if (enabledSides.HasFlag(sideDirection))
                                {
                                    float deltaY = evt.delta.y;
                                    if (Position.height + deltaY < minSize.y)
                                        deltaY = 0;
                                    position.height += deltaY;
                                }
                                break;
                            case UIDirections.Left:
                                if (enabledSides.HasFlag(sideDirection))
                                {
                                    float deltaX = evt.delta.x;
                                    if (position.x + deltaX > position.x + position.width)
                                        deltaX = 0;
                                    position.x += deltaX;
                                    position.width -= deltaX;
                                }
                                break;
                            case UIDirections.Right:
                                if (enabledSides.HasFlag(sideDirection))
                                {
                                    float deltaX = evt.delta.x;
                                    if (position.width + deltaX < minSize.x)
                                        deltaX = 0;
                                    position.width += deltaX;
                                }
                                break;
                            case UIDirections.TopLeft:
                                if (enabledSides.HasFlag(sideDirection))
                                {
                                    float deltaY = evt.delta.y;
                                    if (Position.y + deltaY > Position.y + Position.height)
                                        deltaY = 0;
                                    position.y += deltaY;
                                    position.height -= deltaY;

                                    float deltaX = evt.delta.x;
                                    if (position.x + deltaX > position.x + position.width)
                                        deltaX = 0;
                                    position.x += deltaX;
                                    position.width -= deltaX;
                                }
                                break;
                            case UIDirections.TopRight:
                                if (enabledSides.HasFlag(sideDirection))
                                {
                                    float deltaY = evt.delta.y;
                                    if (Position.y + deltaY > Position.y + Position.height)
                                        deltaY = 0;
                                    position.y += deltaY;
                                    position.height -= deltaY;

                                    float deltaX = evt.delta.x;
                                    if (position.width + deltaX < minSize.x)
                                        deltaX = 0;
                                    position.width += deltaX;
                                }
                                break;
                            case UIDirections.BottomLeft:
                                if (enabledSides.HasFlag(sideDirection))
                                {
                                    float deltaY = evt.delta.y;
                                    if (Position.height + deltaY < minSize.y)
                                        deltaY = 0;
                                    position.height += deltaY;

                                    float deltaX = evt.delta.x;
                                    if (position.x + deltaX > position.x + position.width)
                                        deltaX = 0;
                                    position.x += deltaX;
                                    position.width -= deltaX;
                                }
                                break;
                            case UIDirections.BottomRight:
                                if (enabledSides.HasFlag(sideDirection))
                                {
                                    float deltaY = evt.delta.y;
                                    if (Position.height + deltaY < minSize.y)
                                        deltaY = 0;
                                    position.height += deltaY;

                                    float deltaX = evt.delta.x;
                                    if (position.width + deltaX < minSize.x)
                                        deltaX = 0;
                                    position.width += deltaX;
                                }
                                break;
                            case UIDirections.MiddleCenter:
                                if (enabledSides.HasFlag(sideDirection))
                                    position.position += evt.delta;
                                break;

                        }

                        evt.Use();
                        Reload();
                    }
                    break;
                default:
                    break;
            }

            if (maxSize != Vector2.zero)
            {
                position.width = Mathf.Min(position.width, maxSize.x);
                position.height = Mathf.Min(position.height, maxSize.y);
            }
        }
    }
}