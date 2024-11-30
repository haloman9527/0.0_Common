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
    public class SharedTransform : SharedObject<Transform>
    {
        public SharedTransform() : base() { }

        public SharedTransform(Transform v) : base(v) { }
    }
}
