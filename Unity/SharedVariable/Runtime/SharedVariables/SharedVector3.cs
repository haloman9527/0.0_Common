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
using UnityEngine;

namespace Moyo.SharedVariable
{
    [Serializable]
    public class SharedVector3 : SharedVariable<Vector3>
    {
        public SharedVector3() : base() { }

        public SharedVector3(Vector3 v) : base(v) { }
    }
}
