#region 注 释
/***
 *
 *  Title: ""
 *  Description: 
 *  Date:
 *  Version:
 *  Writer: 半只龙虾人
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */
#endregion
using System;

namespace CZToolKit.Common.ObjectPool
{
    [Serializable]
    public class SimpleObjectPool<T> : ObjectPool<T> where T : class
    {
        public Func<T> generateFunction;
        public Action<T> relesaseAction;
        public Action<T> onSpawn;
        public Action<T> onRecycle;

        protected override T Generate()
        {
            return generateFunction();
        }

        protected override void Release(T unit)
        {
            relesaseAction?.Invoke(unit);
        }

        protected override void OnSpawn(T unit)
        {
            onSpawn?.Invoke(unit);
        }

        protected override void OnRecycle(T unit)
        {
            onRecycle?.Invoke(unit);
        }
    }
}