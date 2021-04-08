using System;
using System.Collections.Generic;
using UnityEngine;

namespace CZToolKit.Core.Blackboards
{
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.HideReferenceObjectPicker]
#endif
    public class Blackboard
    {
        [SerializeField]
        protected Dictionary<string, IBlackboardProperty> blackboard = new Dictionary<string, IBlackboardProperty>();

        public virtual IEnumerable<IBlackboardProperty> GetParams()
        {
            foreach (var property in blackboard)
            {
                yield return property.Value;
            }
        }

        public virtual bool TryGetData<T>(string _name, out T _value, T _fallback = default)
        {
            if (blackboard.TryGetValue(_name, out IBlackboardProperty property))
            {
                if (property is BlackboardProperty<T> tProperty)
                {
                    _value = tProperty.TValue;
                    return true;
                }
            }

            _value = _fallback;
            return false;
        }

        public virtual bool Contains(string _name)
        {
            return blackboard.ContainsKey(_name);
        }

        public virtual bool Contains<T>(string _name)
        {
            if (blackboard.TryGetValue(_name, out IBlackboardProperty property))
            {
                if (property is BlackboardProperty<T> tProperty)
                    return true;
            }
            return false;
        }

        public virtual void SetData<T>(string _name, T _value)
        {
            if (blackboard.TryGetValue(_name, out IBlackboardProperty property))
            {
                if (property is BlackboardProperty<T> tProperty)
                    tProperty.TValue = _value;
            }
            else
            {
                BlackboardProperty<T> tProperty = new BlackboardProperty<T>();
                tProperty.TValue = _value;
                tProperty.Name = _name;
                blackboard[_name] = tProperty;
            }
        }

        public virtual bool TryGetParam(string _name, out IBlackboardProperty _param)
        {
            if (blackboard.TryGetValue(_name, out _param))
                return true;
            _param = null;
            return false;
        }

        public virtual bool Rename(string _oldName, string _newName)
        {
            if (blackboard.TryGetValue(_oldName, out IBlackboardProperty property)) { Debug.LogError($"{_oldName}不被包含在黑板数据内"); return false; }
            if (string.IsNullOrEmpty(_newName)) return false;
            if (blackboard.ContainsKey(_newName)) { Debug.LogError($"黑板内已存在同名数据{_newName}"); return false; }

            blackboard[_newName] = blackboard[_oldName];
            blackboard.Remove(_oldName);
            property.Name = _newName;
            return true;
        }

        public virtual void RemoveData(string _name)
        {
            if (string.IsNullOrEmpty(_name)) return;

            if (blackboard.ContainsKey(_name))
                blackboard.Remove(_name);
        }

        public virtual void Clear()
        {
            blackboard.Clear();
        }
    }

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.HideReferenceObjectPicker]
#endif
    public interface IBlackboardProperty
    {
        string Name { get; set; }
        object Value { get; set; }
        Type PropertyType { get; }
    }

    [Serializable]
    public class BlackboardProperty<T> : IBlackboardProperty
    {
        [SerializeField]
        string name;
        [SerializeField]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.HideReferenceObjectPicker]
#endif
        T value;

        public string Name
        {
            get { return name; }
            set { if (!string.IsNullOrEmpty(value)) name = value; }
        }
        public Type PropertyType => typeof(T);

        public object Value
        {
            get { return value; }
            set
            {
                if (value == null)
                    this.value = default(T);
                else if (value.GetType() == PropertyType)
                    this.value = (T)value;
            }
        }

        public T TValue { get { return value; } set { this.value = value; } }
    }
}
