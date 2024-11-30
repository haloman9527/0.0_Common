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

namespace Moyo
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Interface, Inherited = true)]
    public abstract class PropertyAttribute : Attribute { }
}
