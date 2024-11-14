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
 *  Github: https://github.com/haloman9527
 *  Blog: https://www.haloman.net/
 *
 */
#endregion
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CZToolKit.SharedVariable
{
    [Serializable]
    public class SharedGameObjectList : SharedObjectList<GameObject>
    {
        public SharedGameObjectList() : base() { value = new List<GameObject>(); }

        public SharedGameObjectList(List<GameObject> v) : base(v) { }
    }
}