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
using System.Collections;
using System.Collections.Generic;

using Object = UnityEngine.Object;

namespace CZToolKit.Core.SharedVariable
{
    public interface ISharedObjectList
    {
        Type GetElementType();

        IList GetList();

        void FillList(List<Object> _other);

        void Clear();
    }

    public abstract class SharedObjectList<T> : SharedVariable<List<T>>, ISharedObjectList where T : Object
    {

        protected SharedObjectList() { }

        protected SharedObjectList(string _guid) : base(_guid) { }

        protected SharedObjectList(List<T> _value) : base(_value) { }

        public Type GetElementType()
        {
            return typeof(T);
        }

        public IList GetList()
        {
            return Value;
        }

        public void FillList(List<Object> _other)
        {
            Clear();
            foreach (var item in _other)
            {
                Value.Add(item as T);
            }
        }

        public void Clear()
        {
            value.Clear();
        }
    }
}
