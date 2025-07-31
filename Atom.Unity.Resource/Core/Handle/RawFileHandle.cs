using System;

namespace Atom
{
    public abstract class RawFileHandleBase : HandleBase, IDisposable
    {
        public abstract event Action<RawFileHandleBase> OnCompleted;

        public abstract byte[] GetRawFileData();

        public abstract string GetRawFileText();

        public abstract string GetRawFilePath();

        public abstract void Release();

        public void Dispose() => this.Release();
    }
}