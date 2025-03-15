﻿using System;
using UnityObject = UnityEngine.Object;

namespace Atom
{
    public abstract class AssetHandle : HandleBase
    {
        public abstract UnityObject Asset { get; }

        public abstract event Action<AssetHandle> OnCompleted;

        public abstract void Release();
    }
}