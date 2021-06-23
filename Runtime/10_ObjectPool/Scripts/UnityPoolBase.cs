using System;
using UnityEngine;

namespace CZToolKit.Core.ObjectPool
{
    [Serializable]
    public abstract class UnityPoolBase<T> : PoolBase<T> where T : UnityEngine.Object
    {
        public int maxCount = 10;

        public UnityPoolBase() { }

        public UnityPoolBase(T template)
        {
            SetTemplate(template);
        }

        /// <summary> 设置样本 </summary>
        public UnityPoolBase<T> SetTemplate(T go)
        {
            base.SetTemplate(go);
            return this;
        }

        /// <summary> 生成 </summary>
        /// 重写以使用Object的判空
        public new T Spawn()
        {
            T unit = null;
            while (IdleList.Count > 0 && unit == null)
            {
                unit = IdleList[0];
                IdleList.RemoveAt(0);
            }

            if (unit == null)
                unit = CreateNewUnit();

            WorkList.Add(unit);
            OnBeforeSpawn(unit);

            IPoolable recyclable;
            if ((recyclable = unit as IPoolable) != null)
                recyclable.OnSpawned();
            OnAfterSpawn(unit);
            return unit;
        }

        /// <summary> 回收 </summary>
        public new void Recycle(T _unit)
        {
            WorkList.Remove(_unit);
            if (_unit != null)
            {
                IdleList.Add(_unit);
                OnBeforeRecycle(_unit);
                IPoolable recyclable;
                if ((recyclable = _unit as IPoolable) != null)
                    recyclable.OnRecycled();
                OnAfterRecycle(_unit);
            }
        }

        protected override T CreateNewUnit()
        {
            return GameObject.Instantiate(template);
        }
    }
}
