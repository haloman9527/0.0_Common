using CZToolKit.Core.Editors;
using UnityEngine;

namespace CZToolKit.Core.Blackboards.Editors
{
    [CustomObjectDrawerAttribute(typeof(CZTypeAttribute))]
    public class CZTypeObjectDrawer : ObjectDrawer
    {
        public override void OnGUI(GUIContent label)
        {
            base.OnGUI(label);
            ICZType c = Value as ICZType;
            c.SetValue(EditorGUILayoutExtension.DrawField(label, c.PropertyType, c.GetValue()));
        }
    }
}
