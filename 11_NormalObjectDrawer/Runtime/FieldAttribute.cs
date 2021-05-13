using System;

namespace CZToolKit.Core
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Interface, Inherited = true)]
    public abstract class FieldAttribute : Attribute { }
}
