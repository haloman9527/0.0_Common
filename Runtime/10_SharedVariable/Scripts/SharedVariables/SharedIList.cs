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
using System;
using System.Collections;
using System.Collections.Generic;

using UnityObject = UnityEngine.Object;

namespace CZToolKit.Core.SharedVariable
{
    public interface ISharedObjectList
    {
        Type GetElementType();

        IList GetList();

        void FillList(List<UnityObject> _other);
    }

    public abstract class SharedObjectList<T> : SharedVariable<List<T>>, ISharedObjectList where T : UnityObject
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

        public void FillList(List<UnityObject> _other)
        {
            if (VariableOwner != null && VariableOwner.GetVariable(GUID) == null)
                VariableOwner.SetVariable(this.Clone() as SharedVariable);
            Value.Clear();
            foreach (var item in _other)
            {
                Value.Add(item as T);
            }
        }
    }
}
