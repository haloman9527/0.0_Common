#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 
 *
 */
#endregion
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CZToolKit.Core.SharedVariable
{
    [Serializable]
    public class SharedGameObjectList : SharedVariable<List<GameObject>>
    {
        public SharedGameObjectList() : base() { value = new List<GameObject>(); }

        public SharedGameObjectList(List<GameObject> _value) : base(_value) { }

        public override object Clone()
        {
            SharedGameObjectList variable = new SharedGameObjectList(new List<GameObject>(Value)) { GUID = this.GUID };
            return variable;
        }
    }
}