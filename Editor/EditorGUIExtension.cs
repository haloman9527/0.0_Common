using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CZToolKit.Core.Editors
{
    public class EditorGUIExtension : MonoBehaviour
    {
        static GUIStyle labelStyle;

        /// <summary> 绘制一个ProgressBar </summary>
        public static float ProgressBar(Rect _rect, float _value, float _minLimit, float _maxLimit, string _text, bool _dragable = true, bool _drawMinMax = false)
        {
            float progress = (_value - _minLimit) / (_maxLimit - _minLimit);

            Rect r = _rect;
            GUI.Box(r, "");
            r.width = _rect.width * progress;
            GUI.DrawTexture(r, EditorStylesExtension.WhiteTexture, ScaleMode.StretchToFill, false, 1, new Color(0.07f, 0.56f, 0.9f, 1), 0, 0);

            if (labelStyle == null)
                labelStyle = new GUIStyle(EditorStyles.label);
            if (_drawMinMax)
            {
                labelStyle.alignment = TextAnchor.MiddleLeft;
                GUI.Label(_rect, _minLimit.ToString());
                labelStyle.alignment = TextAnchor.MiddleRight;
                GUI.Label(_rect, _maxLimit.ToString(), labelStyle);
            }
            labelStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Label(_rect, _text, labelStyle);

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
