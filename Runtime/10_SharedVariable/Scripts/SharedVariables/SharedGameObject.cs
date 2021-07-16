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

namespace CZToolKit.Core.SharedVariable
{
    [Serializable]
    public class SharedGameObject : SharedObject<GameObject>
    {
        public SharedGameObject() : base() { }

        public SharedGameObject(GameObject _value) : base(_value) { }

        public override object Clone()
        {
            SharedGameObject variable = new SharedGameObject(Value) { GUID = this.GUID };
            return variable;
        }
    }
}
