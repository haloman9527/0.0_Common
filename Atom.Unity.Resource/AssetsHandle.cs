using System;
using UnityObject = UnityEngine.Object;

namespace Atom
{
    public abstract class AssetsHandleBase : HandleBase
    {
        public abstract UnityObject[] Assets { get; }

        public abstract event Action<AssetsHandleBase> OnCompleted;

        public abstract void Release();

        public void Dispose()
        {
            this.Release();
        }
    }
}