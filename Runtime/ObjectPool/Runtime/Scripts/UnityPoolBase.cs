using System;
using UnityEngine;

namespace CZToolKit.Core.ObjectPool
{
    [Serializable]
    public abstract class UnityPoolBase<T> : Pool<T> where T : UnityEngine.Object
    {
        public int maxCount = 10;

        public UnityPoolBase() { }

        public UnityPoolBase(T template)
        {
            SetTemplate(template);
        }

        /// <summary> 获取 </summary>
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
            IRecyclable recyclable;
            if ((recyclable = unit as IRecyclable) != null)
                recyclable.OnSpawn();
            OnSpawn(unit);
            return unit;
        }

        /// <summary> 回收 </summary>
        /// <param name="unit"></param>
        public override void Recycle(T unit)
        {
            //重写是为了使用被Unity重写过的判空
            if (!WorkList.Contains(unit))
                return;
            WorkList.Remove(unit);
            if (unit != null)
            {
                IdleList.Add(unit);
                IRecyclable recyclable;
                if ((recyclable = unit as IRecyclable) != null)
                    recyclable.OnRecycle();
                OnRecycle(unit);
            }
        }

        /// <summary> 回收所有 </summary>
        /// <param name="unit"></param>
        public override void RecycleAll()
        {
            while (WorkList.Count > 0)
            {
                Recycle(WorkList[0]);
            }
        }

        protected override T CreateNewUnit()
        {
            T go = GameObject.Instantiate(template);
            return go;
        }

        /// <summary>
        /// 设置样本
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public UnityPoolBase<T> SetTemplate(T go)
        {
            base.SetTemplate(go);
            return this;
        }
    }
}
