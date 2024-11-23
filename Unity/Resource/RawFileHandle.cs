using System;

namespace Jiange
{
    public abstract class RawFileHandle : HandleBase
    {
        public abstract event Action<RawFileHandle> OnCompleted;

        public abstract byte[] GetRawFileData();

        public abstract string GetRawFileText();

        public abstract string GetRawFilePath();

        public abstract void Release();
    }
}