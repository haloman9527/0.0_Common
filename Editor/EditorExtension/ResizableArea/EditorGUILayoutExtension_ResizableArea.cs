using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace CZToolKit.Core.Editors
{
    public static partial class EditorGUILayoutExtension
    {
        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static Object[] DragDropAreaMulti(params GUILayoutOption[] _options)
        {
            Rect rect = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.label, _options);
            return EditorGUIExtension.DragDropAreaMulti(rect);
        }

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static Object[] DragDropAreaMulti(DragAndDropVisualMode _dropVisualMode, params GUILayoutOption[] _options)
        {
            Rect rect = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.label, _options);
            return EditorGUIExtension.DragDropAreaMulti(rect, _dropVisualMode);
        }

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static Object[] DragDropAreaMulti(Color _hightlightColor, params GUILayoutOption[] _options)
        {
            Rect rect = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.label, _options);
            return EditorGUIExtension.DragDropAreaMulti(rect, _hightlightColor);
        }

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static Object[] DragDropAreaMulti(DragAndDropVisualMode _dropVisualMode, Color _hightlightColor, params GUILayoutOption[] _options)
        {
            Rect rect = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.label, _options);
            return EditorGUIExtension.DragDropAreaMulti(rect, _dropVisualMode, _hightlightColor);
        }

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static Object DragDropAreaSingle(params GUILayoutOption[] _options)
        {
            Rect rect = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.label, _options);
            return EditorGUIExtension.DragDropAreaSingle(rect);
        }

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static Object DragDropAreaSingle(DragAndDropVisualMode _dropVisualMode, params GUILayoutOption[] _options)
        {
            Rect rect = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.label, _options);
            return EditorGUIExtension.DragDropAreaSingle(rect, _dropVisualMode);
        }

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static Object DragDropAreaSingle(Color _hightlightColor, params GUILayoutOption[] _options)
        {
            Rect rect = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.label, _options);
            return EditorGUIExtension.DragDropAreaSingle(rect, _hightlightColor);
        }

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static Object DragDropAreaSingle(Rect _rect, DragAndDropVisualMode _dropVisualMode, Color _hightlightColor, params GUILayoutOption[] _options)
        {
            Rect rect = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.label, _options);
            return EditorGUIExtension.DragDropAreaSingle(rect, _dropVisualMode, _hightlightColor);
        }
    }
}
