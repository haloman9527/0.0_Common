using System;
using System.Reflection;
using UnityEngine;

namespace CZToolKit.Core.Editors
{
    public class ObjectDrawer
    {
        FieldInfo fieldInfo;
        ObjectDrawerAttribute attribute;
        object value;

        public FieldInfo FieldInfo
        {
            get { return this.fieldInfo; }
            set { this.fieldInfo = value; }
        }

        public ObjectDrawerAttribute Attribute
        {
            get { return this.attribute; }
            set { this.attribute = value; }
        }

        public object Value
        {
            get { return this.value; }
            set
            {
                this.value = value;
                ValueType = value.GetType();
            }
        }

        public Type ValueType { get; private set; }

        public virtual void OnGUI(GUIContent label) { }
    }
}