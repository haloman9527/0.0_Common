using System;
using UnityEngine;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZBlackboardType : CZType<CZBlackboard>
    {
        public CZBlackboardType() : base()
        { Value = null; }

        public CZBlackboardType(CZBlackboard _value) : base(_value) { }

        public static implicit operator CZBlackboardType(CZBlackboard _other) { return new CZBlackboardType(_other); }
    }
}
