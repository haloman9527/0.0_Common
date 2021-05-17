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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Object = UnityEngine.Object;

namespace CZToolKit.Core.SharedVariable
{
    public interface ISharedObject
    {
        Object GetObject();
        void SetObject(Object _object);
    }

    public abstract class SharedObject<T> : SharedVariable<T>, ISharedObject where T : Object
    {
        protected SharedObject() { }

        protected SharedObject(string _guid) : base(_guid) { }

        protected SharedObject(T _value) : base(_value) { }

        public Object GetObject()
        {
            return Value;
        }

        public void SetObject(Object _object)
        {
            Value = _object as T;
        }
    }
}