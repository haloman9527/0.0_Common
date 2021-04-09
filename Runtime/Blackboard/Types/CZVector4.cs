using System;
using UnityEngine;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZVector4 : CZType<Vector4>
    {
        public CZVector4() : base()
        { value = Vector4.zero; }
    }
}
