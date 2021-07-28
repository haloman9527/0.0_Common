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
using UnityEngine;

namespace CZToolKit.Core.SharedVariable
{
    [Serializable]
    [SharedVariable]
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.HideReferenceObjectPicker]
#endif
    public abstract class SharedVariable : ICloneable
    {
        [SerializeField, HideInInspector]
        protected string guid;

        [NonSerialized]
        IVariableOwner variableOwner;

        public string GUID
        {
            get { return this.guid; }
            protected set { this.guid = value; }
        }

        public IVariableOwner VariableOwner
        {
            get { return variableOwner; }
            protected set { variableOwner = value; }
        }

        public SharedVariable() { guid = Guid.NewGuid().ToString(); }

        public SharedVariable(string _guid) { guid = _guid; }

        public virtual void InitializePropertyMapping(IVariableOwner _variableOwner) { }

        public abstract object GetValue();

        public abstract void SetValue(object value);

        public abstract Type GetValueType();

        public abstract object Clone();
    }

    [Serializable]
    public abstract class SharedVariable<T> : SharedVariable
    {
        [SerializeField]
        protected T value;

        [NonSerialized]
        Func<T> getter;
        [NonSerialized]
        Action<T> setter;

        public T Value
        {
            get
            {
                return this.getter == null ? this.value : this.getter();
            }
            set
            {
                if (this.setter != null)
                    this.setter(value);
                else
                    this.value = value;
            }
        }

        protected SharedVariable() : base() { value = default; }

        public SharedVariable(string _guid) : base(_guid) { value = default; }

        public SharedVariable(T _value) : base() { value = _value; }

        public override void InitializePropertyMapping(IVariableOwner _variableOwner)
        {
            VariableOwner = _variableOwner;
            if (VariableOwner == null)
            {
                getter = null;
                setter = null;
                return;
            }

            getter = () =>
            {
                SharedVariable<T> variable = _variableOwner.GetVariable(GUID) as SharedVariable<T>;
                if (variable != null) { return variable.Value; }
                return value;
            };
            setter = _value =>
            {
                SharedVariable variable = VariableOwner.GetVariable(GUID);
                if (variable == null)
                {
                    variable = this.Clone() as SharedVariable;
                    VariableOwner.SetVariable(variable);
                }
                variable.SetValue(_value);
            };
        }

        public override object GetValue()
        {
            if (getter != null)
                return getter();
            else
                return value;
        }

        public override void SetValue(object _value)
        {
            if (setter != null)
                setter((T)_value);
            else
                value = (T)_value;
        }

        public override Type GetValueType() { return typeof(T); }

        public override string ToString()
        {
            string result;
            if (Value == null)
                result = "(null)";
            else
            {
                T value = this.Value;
                result = value.ToString();
            }
            return result;
        }
    }
}
