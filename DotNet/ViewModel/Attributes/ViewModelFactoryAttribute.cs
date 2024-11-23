using System;

namespace Jiange
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ViewModelFactoryAttribute : Attribute
    {
        public Type modelType;

        public ViewModelFactoryAttribute(Type modelType)
        {
            this.modelType = modelType;
        }
    }
}
