using System;
using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace Atom
{
    public abstract class AssetsHandleBase : HandleBase
    {
        public abstract IReadOnlyList<UnityObject> Assets { get; }

        public abstract event Action<AssetsHandleBase> OnCompleted;

        public abstract void Release();

        public void Dispose() => this.Release();
    }
}