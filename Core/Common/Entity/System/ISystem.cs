using System;

namespace CZToolKit
{
    public interface ISystem
    {
        Type EntityType();
        
        Type SystemType();
    }
}