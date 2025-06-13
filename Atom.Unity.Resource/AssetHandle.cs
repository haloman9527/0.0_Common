using System;
using UnityObject = UnityEngine.Object;

namespace Atom
{
    public abstract class AssetHandleBase : HandleBase, IDisposable
    {
        public abstract UnityObject Asset { get; }

        public abstract event Action<AssetHandleBase> OnCompleted;

        public abstract void Release();

        public void Dispose()
        {
            this.Release();
        }
    }
}