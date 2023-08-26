using System;

namespace CZToolKit.VM
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ViewModelAttribute : Attribute
    {
        public Type targetType;

        public ViewModelAttribute(Type targetType)
        {
            this.targetType = targetType;
        }
    }
}
