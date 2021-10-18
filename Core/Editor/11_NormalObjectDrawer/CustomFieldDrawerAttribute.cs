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
using System;

namespace CZToolKit.Core.Editors
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class CustomFieldDrawerAttribute : Attribute
    {
        Type type;

        public Type Type { get { return type; } }

        public CustomFieldDrawerAttribute(Type _type) { type = _type; }
    }
}
#endif