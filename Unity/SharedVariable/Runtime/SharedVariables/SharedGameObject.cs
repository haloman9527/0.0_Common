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

namespace Jiange.SharedVariable
{
    [Serializable]
    public class SharedGameObject : SharedObject<GameObject>
    {
        public SharedGameObject() : base() { }

        public SharedGameObject(GameObject v) : base(v) { }
    }
}
