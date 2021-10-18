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
using System.Collections.Generic;
using UnityEngine;

namespace CZToolKit.Core.SharedVariable
{
    [Serializable]
    public class SharedTransformList : SharedObjectList<Transform>
    {
        public SharedTransformList() : base() { }

        public SharedTransformList(List<Transform> _value) : base(_value) { }

        public override object Clone()
        {
            SharedTransformList variable = new SharedTransformList(new List<Transform>(Value)) { GUID = this.GUID };
            return variable;
        }
    }
}
