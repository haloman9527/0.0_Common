using System;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZChar : CZType<char>
    {
        public CZChar() : base()
        { value = ' '; }
    }
}
