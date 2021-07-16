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
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */
#endregion
#region <<版 本 注 释>>
/***
 *
 *  Title: ""
 *      主题: 对象池基类，只有最基本的功能
 *  Description: 
 *      功能: 
 *      	1.获得一个对象
 *      	2.回收一个对象
 *      	3.执行清洁，移除无用对象
 *  
 *  Date: 
 *  Version: 
 *  Writer:  
 *
 */
#endregion
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CZToolKit.Core.ObjectPool
{
    public interface IPoolBase : IDisposable
    {
        void RecycleAll();
    }

    [Serializable]
    public abstract class PoolBase<T> : IPoolBase where T : class
    {
        [SerializeField]
        protected T template;

        List<T> workList = new List<T>(32);
        List<T> idleList = new List<T>(32);

        public T Template { get { return template; } }

        public List<T> WorkList
        {
            get
            {
                if (workList == null)
                    workList = new List<T>(32);
                return workList;
            }
        }

        public List<T> IdleList
        {
            get
            {
                if (idleList == null)
                    idleList = new List<T>(32);
                return idleList;
            }
        }

        protected virtual void SetTemplate(T _template, bool _replace = false)
        {
            if (this.template == null || _replace && this.template != _template)
                this.template = _template;
        }

        /// <summary> 生成 </summary>
        public T Spawn()
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

        protected virtual void OnBeforeSpawn(T _unit) { }

        protected virtual void OnAfterSpawn(T _unit) { }

        /// <summary> 回收 </summary>
        public void Recycle(T _unit)
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

        protected virtual void OnBeforeRecycle(T _unit) { }

        protected virtual void OnAfterRecycle(T _unit) { }

        /// <summary> 回收所有 </summary>
        public virtual void RecycleAll()
        {
            while (WorkList.Count > 0)
            {
                if (workList[0] != null)
                    Recycle(workList[0]);
                else
                    workList.RemoveAt(0);
            }
        }

        protected abstract T CreateNewUnit();

        public abstract void Dispose();
    }
}