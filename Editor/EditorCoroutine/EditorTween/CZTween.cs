#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 
 *
 */
#endregion
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace CZToolKit.Core.Editors
{
    public class CZTween
    {
        public static void To(Func<float> _getter, Action<float> _setter, float _endValue, float _duration, EasingType _easing = EasingType.Linear)
        {
            GlobalEditorCoroutineMachine.StartCoroutine(FloatTo(_getter, _setter, _endValue, _duration, _easing));
        }

        static IEnumerator FloatTo(Func<float> _getter, Action<float> _setter, float _endValue, float _duration, EasingType _easing = EasingType.Linear)
        {
            float startValue = _getter();
            double startTime = EditorApplication.timeSinceStartup;
            float progress = 0;
            while (progress < 1)
            {
                progress = (float)(EditorApplication.timeSinceStartup - startTime) / _duration;

                float f = 0;
                f = progress / 2 / 0.5f;
                if (progress > 0.5f)
                    f = 1 - progress;
                _setter(Easing.Tween(startValue, _endValue, Mathf.Clamp01(f), _easing));
                yield return null;
            }
        }
    }
}
