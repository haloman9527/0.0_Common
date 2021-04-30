using System;

namespace CZToolKit.Core.Editors
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class CustomObjectDrawerAttribute : Attribute
    {
        Type type;

        public Type Type { get { return type; } }

        public CustomObjectDrawerAttribute(Type _type) { type = _type; }
    }
}