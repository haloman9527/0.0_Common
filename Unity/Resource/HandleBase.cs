using System;
using System.Collections;
using System.Threading.Tasks;

namespace Atom
{
    public abstract class HandleBase
    {
        private event Action<AssetHandle> callback;

        public abstract IEnumerator Enumerator { get; }

        public abstract Task Task { get; }

        public abstract bool IsDone { get; }

        public abstract bool IsValid { get; }
    }
}