using System;

namespace CZToolKit
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