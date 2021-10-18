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
    public class CZSprite : CZType<Sprite>
    {
        public CZSprite() : base()
        { Value = null; }

        public CZSprite(Sprite _value) : base(_value) { }

        public static implicit operator CZSprite(Sprite _other) { return new CZSprite(_other); }
    }
}
