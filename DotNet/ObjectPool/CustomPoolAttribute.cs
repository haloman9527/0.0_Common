using System;

namespace Moyo
{
    public class CustomPoolAttribute : Attribute
    {
        public readonly Type unitType;

        public CustomPoolAttribute(Type unitType)
        {
            this.unitType = unitType;
        }
    }
}