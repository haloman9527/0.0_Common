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

namespace CZToolKit.Core.Editors
{
    public static partial class EditorGUIExtension
    {
        static readonly DragAndDropVisualMode DropVisualMode = DragAndDropVisualMode.Copy;
        static readonly Color DragDropHighlightColor = new Color(0f, 1f, 1f, 0.3f);

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static Object[] DragDropAreaMulti(Rect position)
        {
            return DragDropAreaMulti(position, DropVisualMode, DragDropHighlightColor);
        }

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static Object[] DragDropAreaMulti(Rect position, DragAndDropVisualMode dropVisualMode)
        {
            return DragDropAreaMulti(position, dropVisualMode, DragDropHighlightColor);
        }

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static Object[] DragDropAreaMulti(Rect position, Color hightlightColor)
        {
            return DragDropAreaMulti(position, DropVisualMode, hightlightColor);
        }

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static Object[] DragDropAreaMulti(Rect position, DragAndDropVisualMode dropVisualMode, Color hightlightColor)
        {
            Event evt = Event.current;

            if (!position.Contains(evt.mousePosition)) return null;

            Object[] temp = null;

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    DragAndDrop.visualMode = dropVisualMode;
                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        temp = DragAndDrop.objectReferences;
                    }
                    Event.current.Use();
                    break;
                case EventType.Repaint:
                    if (DragAndDrop.visualMode == dropVisualMode)
                        EditorGUI.DrawRect(position, hightlightColor);
                    break;
                default:
                    break;
            }

            return temp;
        }

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static Object DragDropAreaSingle(Rect position)
        {
            return DragDropAreaSingle(position, DropVisualMode, DragDropHighlightColor);
        }

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static Object DragDropAreaSingle(Rect position, DragAndDropVisualMode dropVisualMode)
        {
            return DragDropAreaSingle(position, dropVisualMode, DragDropHighlightColor);
        }

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static Object DragDropAreaSingle(Rect position, Color hightlightColor)
        {
            return DragDropAreaSingle(position, DropVisualMode, hightlightColor);
        }

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static Object DragDropAreaSingle(Rect position, DragAndDropVisualMode dropVisualMode, Color hightlightColor)
        {
            Object[] temp = DragDropAreaMulti(position, dropVisualMode, hightlightColor);
            if (temp == null || temp.Length == 0) return null;

            for (int i = temp.Length - 1; i >= 0; i--)
            {
                if (temp[i] != null) return temp[i];
            }

            return null;
        }
    }
}
#endif