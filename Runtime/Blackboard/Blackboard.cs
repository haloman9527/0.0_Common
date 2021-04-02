using System;
using System.Collections.Generic;
using UnityEngine;

namespace CZToolKit.Core.Blackboards
{
    public class Blackboard
    {
        [SerializeField]
        protected Dictionary<string, IBlackboardProperty> data = new Dictionary<string, IBlackboardProperty>();

        public virtual bool TryGetData<T>(string _name, out T _value, T _fallback = default)
        {
            if (data.TryGetValue(_name, out IBlackboardProperty property))
            {
                if (property is BlackboardProperty<T> tProperty)
                {
                    _value = tProperty.Value;
                    return true;
                }
            }

            _value = _fallback;
            return false;
        }

        public virtual bool Contains(string _name)
        {
            return data.ContainsKey(_name);
        }

        public virtual bool Contains<T>(string _name)
        {
            if (data.TryGetValue(_name, out IBlackboardProperty property))
            {
                if (property is BlackboardProperty<T> tProperty)
                    return true;
            }
            return false;
        }

        public virtual void SetData<T>(string _name, T _value)
        {
            if (data.TryGetValue(_name, out IBlackboardProperty property))
            {
                if (property is BlackboardProperty<T> tProperty)
                    tProperty.Value = _value;
            }
            else
            {
                BlackboardProperty<T> tProperty = new BlackboardProperty<T>();
                tProperty.Value = _value;
                data[_name] = tProperty;
            }
        }

        public virtual bool Rename(string _oldName, string _newName)
        {
            if (!data.ContainsKey(_oldName)) { Debug.LogError($"{_oldName}不被包含在黑板数据内"); return false; }
            if (string.IsNullOrEmpty(_newName)) return false;
            if (data.ContainsKey(_newName)) { Debug.LogError($"黑板内已存在同名数据{_newName}"); return false; }

            data[_newName] = data[_oldName];
            data.Remove(_oldName);
            return true;
        }

        public virtual void RemoveData(string _name)
        {
            if (string.IsNullOrEmpty(_name)) return;

            if (data.ContainsKey(_name))
                data.Remove(_name);
        }

        public virtual void Clear()
        {
            data.Clear();
        }
    }

    public interface IBlackboardProperty
    {
        Type PropertyType { get; }
    }

    public class BlackboardProperty<T> : IBlackboardProperty
    {
        T value;

        public Type PropertyType => typeof(T);

        public T Value { get { return value; } set { this.value = value; } }
    }
}
