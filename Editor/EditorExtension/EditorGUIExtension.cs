using UnityEditor;
using UnityEngine;

namespace CZToolKit.Core.Editors
{
    public static class EditorGUIExtension
    {
        /// <summary> 绘制一个ProgressBar </summary>
        public static float ProgressBar(Rect _rect, float _value, float _minLimit, float _maxLimit, string _text, bool _dragable = true, bool _drawMinMax = false)
        {
            float progress = (_value - _minLimit) / (_maxLimit - _minLimit);

            Rect r = _rect;
            GUI.Box(r, "");
            r.width = _rect.width * progress;
            EditorGUI.DrawRect(r, new Color(0.07f, 0.56f, 0.9f, 1));

            if (_drawMinMax)
            {
                EditorGUI.LabelField(_rect, _minLimit.ToString());
                EditorGUI.LabelField(_rect, _maxLimit.ToString(), EditorStylesExtension.RightLabelStyle);
            }
            EditorGUI.LabelField(_rect, _text, EditorStylesExtension.MiddleLabelStyle);

            if (_dragable)
#if UNITY_2019_1_OR_NEWER
                return GUI.HorizontalSlider(_rect, _value, _minLimit, _maxLimit, EditorStylesExtension.Transparent, EditorStylesExtension.Transparent, EditorStylesExtension.Transparent);
#else
                return GUI.HorizontalSlider(_rect, _value, _minLimit, _maxLimit, EditorStylesExtension.Transparent, EditorStylesExtension.Transparent);
#endif
            return _value;
        }

        static readonly DragAndDropVisualMode dropVisualMode = DragAndDropVisualMode.Copy;
        static readonly Color dragDropHighlightColor = new Color(0f, 1f, 1f, 0.3f);

        public static DragAndDropVisualMode DropVisualMode { get { return dropVisualMode; } }
        public static Color DragDropHighlightColor { get { return dragDropHighlightColor; } }

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static Object[] DragDropAreaMulti(Rect _rect)
        {
            return DragDropAreaMulti(_rect, DropVisualMode, DragDropHighlightColor);
        }

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static Object[] DragDropAreaMulti(Rect _rect, DragAndDropVisualMode _dropVisualMode)
        {
            return DragDropAreaMulti(_rect, _dropVisualMode, DragDropHighlightColor);
        }

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static Object[] DragDropAreaMulti(Rect _rect, Color _hightlightColor)
        {
            return DragDropAreaMulti(_rect, DropVisualMode, _hightlightColor);
        }

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static Object[] DragDropAreaMulti(Rect _rect, DragAndDropVisualMode _dropVisualMode, Color _hightlightColor)
        {
            Event evt = Event.current;

            if (!_rect.Contains(evt.mousePosition)) return null;

            Object[] temp = null;

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    DragAndDrop.visualMode = _dropVisualMode;
                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        temp = DragAndDrop.objectReferences;
                    }
                    Event.current.Use();
                    break;
                case EventType.Repaint:
                    if (DragAndDrop.visualMode == _dropVisualMode)
                        EditorGUI.DrawRect(_rect, _hightlightColor);
                    break;
                default:
                    break;
            }

            return temp;
        }

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static Object DragDropAreaSingle(Rect _rect)
        {
            return DragDropAreaSingle(_rect, DropVisualMode, DragDropHighlightColor);
        }

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static Object DragDropAreaSingle(Rect _rect, DragAndDropVisualMode _dropVisualMode)
        {
            return DragDropAreaSingle(_rect, _dropVisualMode, DragDropHighlightColor);
        }

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static Object DragDropAreaSingle(Rect _rect, Color _hightlightColor)
        {
            return DragDropAreaSingle(_rect, DropVisualMode, _hightlightColor);
        }

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static Object DragDropAreaSingle(Rect _rect, DragAndDropVisualMode _dropVisualMode, Color _hightlightColor)
        {
            Object[] temp = DragDropAreaMulti(_rect, _dropVisualMode, _hightlightColor);
            if (temp == null || temp.Length == 0) return null;

            for (int i = temp.Length - 1; i >= 0; i--)
            {
                if (temp[i] != null) return temp[i];
            }

            return null;
        }
    }
}
