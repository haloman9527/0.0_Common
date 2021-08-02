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
using System;
using System.Reflection;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace CZToolKit.Core.Editors
{
    public class FieldDrawer
    {
        FieldInfo fieldInfo;
        FieldAttribute attribute;
        object value;

        public FieldInfo FieldInfo
        {
            get { return this.fieldInfo; }
            set { this.fieldInfo = value; }
        }

        public FieldAttribute Attribute
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
                ValueType = this.value == null ? typeof(object) : this.value.GetType();
            }
        }

        public Type ValueType { get; private set; }

        public virtual void OnGUI(GUIContent label) { }
    }
}