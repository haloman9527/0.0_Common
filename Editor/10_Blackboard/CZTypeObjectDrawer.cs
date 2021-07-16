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
using CZToolKit.Core.Editors;
using UnityEngine;

namespace CZToolKit.Core.Blackboards.Editors
{
    [CustomFieldDrawerAttribute(typeof(CZTypeAttribute))]
    public class CZTypeObjectDrawer : FieldDrawer
    {
        public override void OnGUI(GUIContent label)
        {
            base.OnGUI(label);
            ICZType c = Value as ICZType;
            c.SetValue(EditorGUILayoutExtension.DrawField(label, c.ValueType, c.GetValue()));
        }
    }
}
