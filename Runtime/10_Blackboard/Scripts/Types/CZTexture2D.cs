#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 半只龙虾人
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */
#endregion
using System;
using UnityEngine;

namespace CZToolKit.Core.Blackboards
{
    [Serializable]
    public class CZTexture2D : CZType<Texture2D>
    {
        public CZTexture2D() : base()
        { Value = null; }

        public CZTexture2D(Texture2D _value) : base(_value) { }

        public static implicit operator CZTexture2D(Texture2D _other) { return new CZTexture2D(_other); }
    }
}
