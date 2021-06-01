#region <<版 本 注 释>>
/***
 *
 *  Title: ""
 *      主题: 事件类,从无参委托到四个参数的事件
 *  Description: 
 *      功能: 
 *      	1.事件基类
 *      	2.无参事件类
 *      	3.一参事件类
 *      	4.二参事件类
 *      	5.三参事件类
 *      	6.四参事件类
 *  Date: 
 *  Version: 
 *  Writer:  
 *
 */
#endregion
using System;

namespace CZToolKit.Core.EventCenter
{
    public class CZEvent : ICZEvent
    {
        public event Action czEvent;

        public void AddListener(Action _action)
        {
            czEvent += _action;
        }

        public void RemoveListener(Action _action)
        {
            czEvent -= _action;
        }

        public void RemoveAllListener()
        {
            while (czEvent != null)
            {
                czEvent = null;
            }
        }

        public void Invoke()
        {
            if (czEvent != null)
                czEvent();
        }
    }

    public class CZEvent<Arg0> : ICZEvent
    {
        public event Action<Arg0> czEvent;

        public void AddListener(Action<Arg0> _action)
        {
            czEvent += _action;
        }

        public void RemoveListener(Action<Arg0> _action)
        {
            czEvent -= _action;
        }

        public void Invoke(Arg0 _arg0)
        {
            if (czEvent != null)
                czEvent(_arg0);
        }

        public void RemoveAllListener()
        {
            while (czEvent != null)
            {
                czEvent = null;
            }
        }
    }

    public class CZEvent<Arg0, Arg1> : ICZEvent
    {
        public event Action<Arg0, Arg1> czEvent;

        public void AddListener(Action<Arg0, Arg1> _action)
        {
            czEvent += _action;
        }

        public void RemoveListener(Action<Arg0, Arg1> _action)
        {
            czEvent -= _action;
        }

        public void Invoke(Arg0 _arg0, Arg1 _arg1)
        {
            if (czEvent != null)
                czEvent(_arg0, _arg1);
        }

        public void RemoveAllListener()
        {
            while (czEvent != null)
            {
                czEvent = null;
            }
        }
    }

    public class CZEvent<Arg0, Arg1, Arg2> : ICZEvent
    {
        public event Action<Arg0, Arg1, Arg2> czEvent;

        public void AddListener(Action<Arg0, Arg1, Arg2> _action)
        {
            czEvent += _action;
        }

        public void RemoveListener(Action<Arg0, Arg1, Arg2> _action)
        {
            czEvent -= _action;
        }

        public void Invoke(Arg0 _arg0, Arg1 _arg1, Arg2 _arg2)
        {
            if (czEvent != null)
                czEvent(_arg0, _arg1, _arg2);
        }

        public void RemoveAllListener()
        {
            while (czEvent != null)
            {
                czEvent = null;
            }
        }
    }

    public class CZEvent<Arg0, Arg1, Arg2, Arg3> : ICZEvent
    {
        public event Action<Arg0, Arg1, Arg2, Arg3> czEvent;

        public void AddListener(Action<Arg0, Arg1, Arg2, Arg3> _action)
        {
            czEvent += _action;
        }

        public void RemoveListener(Action<Arg0, Arg1, Arg2, Arg3> _action)
        {
            czEvent -= _action;
        }

        public void Invoke(Arg0 _arg0, Arg1 _arg1, Arg2 _arg2, Arg3 _arg3)
        {
            if (czEvent != null)
                czEvent(_arg0, _arg1, _arg2, _arg3);
        }

        public void RemoveAllListener()
        {
            while (czEvent != null)
            {
                czEvent = null;
            }
        }
    }
}
