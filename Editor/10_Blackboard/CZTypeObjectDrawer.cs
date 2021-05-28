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
