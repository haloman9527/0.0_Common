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
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace CZToolKit.Core.Editors
{
    public static partial class EditorGUIExtension
    {
        static Stack<Font> fonts = new Stack<Font>();
        public static void BeginFont(Font _font)
        {
            fonts.Push(GUI.skin.font);
            GUI.skin.font = _font;
        }

        public static void EndFont()
        {
            GUI.skin.font = fonts.Pop();
        }

        static Stack<Color> colors = new Stack<Color>();
        public static void BeginColor(Color _color)
        {
            colors.Push(GUI.color);
            GUI.color = _color;
        }

        public static void EndColor()
        {
            GUI.color = colors.Pop();
        }

        public static void BeginAlpha(float alpha)
        {
            Color color = GUI.color;
            color.a *= alpha;
            BeginColor(color);
        }

        public static void EndAlpha()
        {
            EndColor();
        }

        static Stack<Matrix4x4> matrixs = new Stack<Matrix4x4>();
        public static void BeginMatrix(Matrix4x4 matrix4X4)
        {
            matrixs.Push(GUI.matrix);
            GUI.matrix = matrix4X4;
        }

        public static void EndMatrix()
        {
            GUI.matrix = matrixs.Pop();
        }

        public static void BeginScale(Vector2 scale, Rect rect, Vector2 pivot)
        {
            Rect Scale(Rect _targetValue, Vector2 _scale, Vector2 _pivot)
            {
                Vector2 absPosition = _targetValue.position + _targetValue.size * _pivot;
                Vector2 size = _targetValue.size;
                size.x *= _scale.x;
                size.y *= _scale.y;
                _targetValue.size = size;
                _targetValue.position = absPosition - _targetValue.size * _pivot;
                return _targetValue;
            }

            Rect r = Scale(rect, scale, pivot);
            Vector2 offset = new Vector2(r.x - rect.x, r.y - rect.y);
            Matrix4x4 matrix = GUI.matrix;
            matrix.m03 += offset.x;
            matrix.m13 += offset.y;
            matrix.m00 *= scale.x;
            matrix.m11 *= scale.y;
            BeginMatrix(matrix);
        }

        public static void EndScale()
        {
            EndMatrix();
        }

        static Stack<Color> backgroundColors = new Stack<Color>();
        public static void BeginBackgroundColor(Color _color)
        {
            backgroundColors.Push(_color);
            GUI.color = _color;
        }

        public static void EndBackgroundColor()
        {
            GUI.color = backgroundColors.Pop();
        }

        /// <summary> 绘制一个ProgressBar </summary>
        public static float ProgressBar(Rect _rect, float _value, float _minLimit, float _maxLimit, string _text, bool _dragable = true, bool _drawMinMax = false)
        {
            float progress = (_value - _minLimit) / (_maxLimit - _minLimit);

            Rect r = _rect;
            GUI.Box(r, string.Empty);
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
    }
}
#endif