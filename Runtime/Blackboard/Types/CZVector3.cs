using System;
using UnityEngine;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZVector3 : CZType<Vector3>
    {
        public CZVector3() : base()
        { value = Vector3.zero; }
    }
}
