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
using UnityEngine;

using Object = UnityEngine.Object;

namespace CZToolKit.Core.ObjectPool
{
    [Serializable]
    public abstract class Pool_Unity<T> : PoolBase<T> where T : Object
    {
        public override T Spawn()
        {
            //重写是为了使用被Unity重写过的判空
            T unit = null;
            while (IdleList.Count > 0 && unit == null)
            {
                unit = IdleList[0];
                IdleList.RemoveAt(0);
            }

            if (unit == null)
                unit = CreateNewUnit();

            WorkList.Add(unit);
            return unit;
        }

        public override void Recycle(T _unit)
        {
            if (_unit == null)
            {
                WorkList.Remove(_unit);
                return;
            }
            base.Recycle(_unit);
        }
    }

    [Serializable]
    public class Pool_UnityGameObject : Pool_Unity<GameObject>
    {
        public override GameObject Spawn()
        {
            GameObject unit = base.Spawn();
            unit.SetActive(true);
            return unit;
        }

        protected override GameObject CreateNewUnit()
        {
            return GameObject.Instantiate(Template);
        }
    }
}
