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
 *  Github: https://github.com/haloman9527
 *  Blog: https://www.haloman.net/
 *
 */
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace Moyo.SharedVariable
{
    public interface ISharedObject
    {
        UnityObject GetObject();
        void SetObject(UnityObject _object);
    }

    public abstract class SharedObject<T> : SharedVariable<T>, ISharedObject where T : UnityObject
    {
        protected SharedObject() { }

        protected SharedObject(T v) : base(v) { }

        public UnityObject GetObject()
        {
            return Value;
        }

        public void SetObject(UnityObject _object)
        {
            Value = _object as T;
        }
    }
}