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

using UnityEngine;

namespace CZToolKit.Core.ObjectPool
{
    public class Pool_CS<T> : PoolBase<T> where T : class, new()
    {
        protected override T CreateNewUnit()
        {
            return new T();
        }
    }
}
