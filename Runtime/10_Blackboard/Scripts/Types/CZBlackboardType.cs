using System;
using UnityEngine;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZBlackboardType : CZType<CZBlackboardAsset>
    {
        public CZBlackboardType() : base()
        { Value = null; }

        public CZBlackboardType(CZBlackboardAsset _value) : base(_value) { }

        public static implicit operator CZBlackboardType(CZBlackboardAsset _other) { return new CZBlackboardType(_other); }
    }
}
