﻿#region <<版 本 注 释>>
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
    [Serializable]
    public abstract class PoolBase : IDisposable
    {
        public PoolBase() { }
        public virtual void Dispose() { }
    }

    [Serializable]
    public abstract class Pool<T> : PoolBase where T : class
    {
        [SerializeField] protected T template;
        List<T> workList = new List<T>(16);
        List<T> idleList = new List<T>(16);

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

        /// <summary> 获取 </summary>
        public virtual T Spawn()
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
            OnSpawn(unit);
            return unit;
        }

        /// <summary> 回收 </summary>
        public virtual void Recycle(T _unit)
        {
            WorkList.Remove(_unit);
            IdleList.Add(_unit);
            OnRecycle(_unit);
        }

        /// <summary> 回收所有 </summary>
        public virtual void RecycleAll()
        {
            foreach (T unit in WorkList)
            {
                IdleList.Add(unit);
                OnRecycle(unit);
            }
            WorkList.Clear();
        }

        protected virtual void SetTemplate(T _template, bool _replace = false)
        {
            if (this.template == null || _replace && this.template != _template)
                this.template = _template;
        }

        /// <summary> 当得到时 </summary>
        /// <param name="unit"></param>
        protected virtual void OnSpawn(T unit) { }

        /// <summary> 当回收时 </summary>
        /// <param name="unit"></param>
        protected virtual void OnRecycle(T unit) { }

        protected abstract T CreateNewUnit();
    }
}