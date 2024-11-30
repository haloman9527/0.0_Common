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

namespace Moyo.SharedVariable
{
    [Serializable]
    public class SharedBool : SharedVariable<bool>
    {
        public SharedBool() : base() { }

        public SharedBool(bool v) : base(v) { }
    }
}
