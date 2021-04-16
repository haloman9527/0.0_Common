using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CZToolKit.Core.Singletons
{
    public interface ICZSingleton
    {
        void OnInitialize();

        void OnClean();
    }
}
