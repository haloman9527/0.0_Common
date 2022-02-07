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
using CZToolKit.Core.Editors;
using UnityEngine;

namespace CZToolKit.Core.Blackboards.Editors
{
    [CustomPropertyDrawer(typeof(CZTypeAttribute))]
    public class CZTypeObjectDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, GUIContent label)
        {
            base.OnGUI(position, label);
            ICZType c = Target as ICZType;
            c.SetValue(EditorGUILayoutExtension.DrawField(c.GetValue(), label));
        }
    }
}
#endif