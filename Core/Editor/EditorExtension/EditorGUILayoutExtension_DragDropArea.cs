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

using UnityObject = UnityEngine.Object;

namespace CZToolKit.Core.Editors
{
    public static partial class EditorGUILayoutExtension
    {
        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static UnityObject[] DragDropAreaMulti(params GUILayoutOption[] options)
        {
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.label, options);
            return EditorGUIExtension.DragDropAreaMulti(rect);
        }

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static UnityObject[] DragDropAreaMulti(DragAndDropVisualMode dropVisualMode, params GUILayoutOption[] options)
        {
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.label, options);
            return EditorGUIExtension.DragDropAreaMulti(rect, dropVisualMode);
        }

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static UnityObject[] DragDropAreaMulti(Color hightlightColor, params GUILayoutOption[] options)
        {
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.label, options);
            return EditorGUIExtension.DragDropAreaMulti(rect, hightlightColor);
        }

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static UnityObject[] DragDropAreaMulti(DragAndDropVisualMode dropVisualMode, Color hightlightColor, params GUILayoutOption[] options)
        {
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.label, options);
            return EditorGUIExtension.DragDropAreaMulti(rect, dropVisualMode, hightlightColor);
        }

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static UnityObject DragDropAreaSingle(params GUILayoutOption[] options)
        {
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.label, options);
            return EditorGUIExtension.DragDropAreaSingle(rect);
        }

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static UnityObject DragDropAreaSingle(DragAndDropVisualMode dropVisualMode, params GUILayoutOption[] options)
        {
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.label, options);
            return EditorGUIExtension.DragDropAreaSingle(rect, dropVisualMode);
        }

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static UnityObject DragDropAreaSingle(Color hightlightColor, params GUILayoutOption[] options)
        {
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.label, options);
            return EditorGUIExtension.DragDropAreaSingle(rect, hightlightColor);
        }

        /// <summary> 绘制一个可接收拖拽资源的区域 </summary>
        public static UnityObject DragDropAreaSingle(DragAndDropVisualMode dropVisualMode, Color hightlightColor, params GUILayoutOption[] options)
        {
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.label, options);
            return EditorGUIExtension.DragDropAreaSingle(rect, dropVisualMode, hightlightColor);
        }
    }
}
#endif