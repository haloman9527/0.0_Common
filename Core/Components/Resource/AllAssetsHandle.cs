﻿using System;

namespace CZToolKit
{
    public abstract class AllAssetsHandle : HandleBase
    {
        public abstract UnityEngine.Object[] AllAssets { get; }
        

        public abstract event Action<AllAssetsHandle> OnCompleted;

        public abstract void Release();
    }
}