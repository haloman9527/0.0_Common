#region <<版 本 注 释>>
/***
 *
 *  Title: ""
 *      主题: 事件管理器,最多支持四个参数
 *  Description: 
 *      功能: 
 *      	1.注册一个事件,从无参到四参
 *      	2.调度一个事件,从无参到四参
 *      	3.移除一个事件
 *      	4.移除一类事件
 *      	5.清空事件列表
 *  
 *  Date: 
 *  Version: 
 *  Writer:  
 *
 */

#endregion

using CZToolKit.Core.Singletons;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CZToolKit.Core.EventCenter
{
    public class CZEventCenter : CZNormalSingleton<CZEventCenter>
    {
        private readonly Dictionary<string, ICZEvent> eventsDic = new Dictionary<string, ICZEvent>();

        #region RegisterEvent

        public bool AddListener(string _evtName, Action _evt)
        {
            if (string.IsNullOrEmpty(_evtName) || _evt == null)
                return false;

            CZEvent czEvt;
            if (eventsDic.TryGetValue(_evtName, out ICZEvent eventBase))
            {
                czEvt = eventBase as CZEvent;
                if (czEvt == null)
                {
                    Debug.LogWarning("已存在参数数量不符的同名事件，故无法新增事件");
                    return false;
                }
            }
            else
            {
                czEvt = new CZEvent();
                eventsDic[_evtName] = czEvt;
            }

            czEvt.AddListener(_evt);
            return true;
        }

        public bool AddListener<Arg0>(string _evtName, Action<Arg0> _evt)
        {
            if (string.IsNullOrEmpty(_evtName) || _evt == null)
                return false;

            CZEvent<Arg0> czEvt;

            if (eventsDic.TryGetValue(_evtName, out ICZEvent eventBase))
            {
                czEvt = eventBase as CZEvent<Arg0>;
                if (czEvt == null)
                {
                    Debug.LogWarning("已存在参数数量不符的同名事件，故无法新增事件");
                    return false;
                }
            }
            else
                eventsDic[_evtName] = czEvt = new CZEvent<Arg0>();

            czEvt.AddListener(_evt);
            return true;
        }

        public bool AddListener<Arg0, Arg1>(string _evtName, Action<Arg0, Arg1> _evt)
        {
            if (string.IsNullOrEmpty(_evtName) || _evt == null)
                return false;

            CZEvent<Arg0, Arg1> czEvt;

            if (eventsDic.TryGetValue(_evtName, out ICZEvent eventBase))
            {
                czEvt = eventBase as CZEvent<Arg0, Arg1>;
                if (czEvt == null)
                {
                    Debug.LogWarning("已存在参数数量不符的同名事件，故无法新增事件");
                    return false;
                }
            }
            else
                eventsDic[_evtName] = czEvt = new CZEvent<Arg0, Arg1>();

            czEvt.AddListener(_evt);
            return true;
        }

        public bool AddListener<Arg0, Arg1, Arg2>(string _evtName, Action<Arg0, Arg1, Arg2> _evt)
        {
            if (string.IsNullOrEmpty(_evtName) || _evt == null)
                return false;

            CZEvent<Arg0, Arg1, Arg2> czEvt;

            if (eventsDic.TryGetValue(_evtName, out ICZEvent eventBase))
            {
                czEvt = eventBase as CZEvent<Arg0, Arg1, Arg2>;
                if (czEvt == null)
                {
                    Debug.LogWarning("已存在参数数量不符的同名事件，故无法新增事件");
                    return false;
                }
            }
            else
                eventsDic[_evtName] = czEvt = new CZEvent<Arg0, Arg1, Arg2>();

            czEvt.AddListener(_evt);
            return true;
        }

        public bool AddListener<Arg0, Arg1, Arg2, Arg3>(string _evtName, Action<Arg0, Arg1, Arg2, Arg3> _evt)
        {
            if (string.IsNullOrEmpty(_evtName) || _evt == null)
                return false;

            CZEvent<Arg0, Arg1, Arg2, Arg3> czEvt;

            if (eventsDic.TryGetValue(_evtName, out ICZEvent eventBase))
            {
                czEvt = eventBase as CZEvent<Arg0, Arg1, Arg2, Arg3>;
                if (czEvt == null)
                {
                    Debug.LogWarning("已存在参数数量不符的同名事件，故无法新增事件");
                    return false;
                }
            }
            else
                eventsDic[_evtName] = czEvt = new CZEvent<Arg0, Arg1, Arg2, Arg3>();

            czEvt.AddListener(_evt);
            return true;
        }

        #endregion

        #region DispatchEvent

        public void DispatchEvent(string _evtName)
        {
            if (string.IsNullOrEmpty(_evtName))
            {
                Debug.Log("事件名不能为空");
                return;
            }

            CZEvent czEvt = null;

            if (eventsDic.TryGetValue(_evtName, out ICZEvent tempEvt))
                czEvt = tempEvt as CZEvent;

            if (czEvt == null)
                Debug.Log(" \"" + _evtName + " \"事件所需参数类型或个数不合理, 或者该事件不存在");
            else
                czEvt.Invoke();
        }

        public void DispatchEvent<Arg0>(string _evtName, Arg0 _arg0)
        {
            if (string.IsNullOrEmpty(_evtName))
            {
                Debug.Log("事件名不能为空");
                return;
            }

            CZEvent<Arg0> czEvt = null;

            if (eventsDic.TryGetValue(_evtName, out ICZEvent tempEvt))
                czEvt = tempEvt as CZEvent<Arg0>;

            if (czEvt != null)
                czEvt.Invoke(_arg0);
            else
                Debug.Log(" \"" + _evtName + " \"事件所需参数类型或个数不合理, 或者该事件不存在");
        }

        public void DispatchEvent<Arg0, Arg1>(string _evtName, Arg0 _arg0, Arg1 _arg1)
        {
            if (string.IsNullOrEmpty(_evtName))
            {
                Debug.Log("事件名不能为空");
                return;
            }

            CZEvent<Arg0, Arg1> czEvt = null;

            if (eventsDic.TryGetValue(_evtName, out ICZEvent tempEvt))
                czEvt = tempEvt as CZEvent<Arg0, Arg1>;

            if (czEvt != null)
                czEvt.Invoke(_arg0, _arg1);
            else
                Debug.Log(" \"" + _evtName + " \"事件所需参数类型或个数不合理, 或者该事件不存在");
        }

        public void DispatchEvent<Arg0, Arg1, Arg2>(string _evtName, Arg0 _arg0, Arg1 _arg1, Arg2 _arg2)
        {
            if (string.IsNullOrEmpty(_evtName))
            {
                Debug.Log("事件名不能为空");
                return;
            }

            CZEvent<Arg0, Arg1, Arg2> czEvt = null;

            if (eventsDic.TryGetValue(_evtName, out ICZEvent tempEvt))
                czEvt = tempEvt as CZEvent<Arg0, Arg1, Arg2>;

            if (czEvt == null)
                Debug.Log(" \"" + _evtName + " \"事件所需参数类型或个数不合理, 或者该事件不存在");
            else
                czEvt.Invoke(_arg0, _arg1, _arg2);
        }

        public void DispatchEvent<Arg0, Arg1, Arg2, Arg3>(string _evtName, Arg0 _arg0, Arg1 _arg1, Arg2 _arg2, Arg3 _arg3)
        {
            if (string.IsNullOrEmpty(_evtName))
            {
                Debug.Log("事件名不能为空");
                return;
            }

            CZEvent<Arg0, Arg1, Arg2, Arg3> czEvt = null;

            if (eventsDic.TryGetValue(_evtName, out ICZEvent tempEvt))
                czEvt = tempEvt as CZEvent<Arg0, Arg1, Arg2, Arg3>;

            if (czEvt == null)
                Debug.Log(" \"" + _evtName + " \"事件所需参数类型或个数不合理, 或者该事件不存在");
            else
                czEvt.Invoke(_arg0, _arg1, _arg2, _arg3);
        }

        #endregion

        #region RemoveListener

        public void RemoveListener(string _evtName, Action _evt)
        {
            if (string.IsNullOrEmpty(_evtName) 
                || _evt == null 
                || !eventsDic.TryGetValue(_evtName, out ICZEvent eventBase))
                return;

            if (eventBase is CZEvent czEvt)
                czEvt.RemoveListener(_evt);
        }

        public void RemoveListener<Arg0>(string _evtName, Action<Arg0> _evt)
        {
            if (string.IsNullOrEmpty(_evtName) || _evt == null || !eventsDic.TryGetValue(_evtName, out ICZEvent eventBase))
                return;

            if (eventBase is CZEvent<Arg0> czEvt)
                czEvt.RemoveListener(_evt);
        }

        public void RemoveListener<Arg0, Arg1>(string _evtName, Action<Arg0, Arg1> _evt)
        {
            if (string.IsNullOrEmpty(_evtName) || _evt == null || !eventsDic.TryGetValue(_evtName, out ICZEvent eventBase))
                return;

            if (eventBase is CZEvent<Arg0, Arg1> czEvt)
                czEvt.RemoveListener(_evt);
        }

        public void RemoveListener<Arg0, Arg1, Arg2>(string _evtName, Action<Arg0, Arg1, Arg2> _evt)
        {
            if (string.IsNullOrEmpty(_evtName) || _evt == null || !eventsDic.TryGetValue(_evtName, out ICZEvent eventBase))
                return;

            if (eventBase is CZEvent<Arg0, Arg1, Arg2> czEvt)
                czEvt.RemoveListener(_evt);
        }

        public void RemoveListener<Arg0, Arg1, Arg2, Arg3>(string _evtName, Action<Arg0, Arg1, Arg2, Arg3> _evt)
        {
            if (string.IsNullOrEmpty(_evtName) || _evt == null || !eventsDic.TryGetValue(_evtName, out ICZEvent eventBase))
                return;

            if (eventBase is CZEvent<Arg0, Arg1, Arg2, Arg3> czEvt)
                czEvt.RemoveListener(_evt);
        }

        #endregion

        public void Clear()
        {
            eventsDic.Clear();
        }
    }
}