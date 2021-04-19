using System;
using UnityEngine;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZBlackboardType : CZType<CZBlackboard>
    {
        public CZBlackboardType() : base()
        { value = null; }
    }
}
