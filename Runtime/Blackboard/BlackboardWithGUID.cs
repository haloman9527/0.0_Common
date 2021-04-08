using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CZToolKit.Core.Blackboards
{
    public class BlackboardWithGUID : Blackboard
    {
        /// <summary> Name与GUID映射表 </summary>
        [SerializeField]
        [HideInInspector]
        protected Dictionary<string, string> guidMap = new Dictionary<string, string>();

        #region GetData
        public override bool TryGetData<T>(string _name, out T _data, T _fallback = default)
        {
            if (guidMap.TryGetValue(_name, out string guid))
            {
                if (TryGetDataFromGUID(guid, out _data, _fallback))
                    return true;
            }

            _data = _fallback;
            return false;
        }

        public override bool TryGetParam(string _name, out IBlackboardProperty _param)
        {
            if (guidMap.TryGetValue(_name, out string guid))
            {
                if (blackboard.TryGetValue(guid, out _param))
                    return true;
            }
            _param = null;
            return false;
        }

        public bool TryGetParamFromGUID(string _guid, out IBlackboardProperty _param)
        {
            return blackboard.TryGetValue(_guid, out _param);
        }

        public bool TryGetDataFromGUID<T>(string _guid, out T _data, T _fallback = default)
        {
            if (blackboard.TryGetValue(_guid, out IBlackboardProperty property))
            {
                if (property is BlackboardPropertyGUID<T> tProperty)
                {
                    _data = tProperty.TValue;
                    return true;
                }
            }

            _data = _fallback;
            return false;
        }
        #endregion

        #region Contains
        public override bool Contains(string _name)
        {
            if (guidMap.TryGetValue(_name, out string guid))
                return ContainsGUID(guid);
            return false;
        }

        public bool ContainsGUID(string _guid)
        {
            return blackboard.ContainsKey(_guid);
        }

        public override bool Contains<T>(string _name)
        {
            if (guidMap.TryGetValue(_name, out string guid))
                return ContainsGUID<T>(guid);
            return false;
        }

        public bool ContainsGUID<T>(string _guid)
        {
            if (blackboard.TryGetValue(_guid, out IBlackboardProperty property))
            {
                if (property is BlackboardPropertyGUID<T> tProperty)
                    return true;
            }
            return false;
        }
        #endregion

        #region Set
        public override void SetData<T>(string _name, T _data)
        {
            if (guidMap.TryGetValue(_name, out string guid))
                SetDataFromGUID(_name, guid, _data);
            else
            {
                BlackboardPropertyGUID<T> tProperty = new BlackboardPropertyGUID<T>();
                tProperty.TValue = _data;
                tProperty.Name = _name;
                SetData(_name, tProperty.GUID, tProperty);
            }
        }

        private void SetDataFromGUID<T>(string _name, string _guid, T _data)
        {
            if (blackboard.TryGetValue(_guid, out IBlackboardProperty property))
            {
                if (property is BlackboardPropertyGUID<T> tProperty)
                    tProperty.TValue = _data;
            }
            else
            {
                BlackboardPropertyGUID<T> tProperty = new BlackboardPropertyGUID<T>();
                tProperty.TValue = _data;
                SetData(_name, _guid, tProperty);
            }
        }

        private void SetData(string _name, string _guid, IBlackboardProperty _tProperty)
        {
            guidMap[_name] = _guid;
            blackboard[_guid] = _tProperty;
        }

        public bool AddProperty(IBlackboardPropertyGUID _property)
        {
            if (guidMap.TryGetValue(_property.Name, out string guid) && guid == _property.GUID && blackboard.ContainsKey(_property.GUID))
                return false;
            guidMap[_property.Name] = _property.GUID;
            blackboard[_property.GUID] = _property;
            return true;
        }
        #endregion

        public bool TryGetGUID(string _name, out string _guid)
        {
            return guidMap.TryGetValue(_name, out _guid);
        }

        public override bool Rename(string _oldName, string _newName)
        {
            if (!guidMap.TryGetValue(_oldName, out string guid)) { Debug.LogWarning($"{_oldName}不被包含在黑板数据内"); return false; }
            if (string.IsNullOrEmpty(_newName)) return false;
            if (guidMap.ContainsKey(_newName)) { Debug.LogWarning($"黑板内已存在同名数据{_newName}"); return false; }

            guidMap[_newName] = guidMap[_oldName];
            if (blackboard.TryGetValue(guid, out IBlackboardProperty property))
                property.Name = _newName;
            guidMap.Remove(_oldName);
            return true;
        }

        #region Remove
        public override void RemoveData(string _name)
        {
            if (string.IsNullOrEmpty(_name)) return;

            if (guidMap.TryGetValue(_name, out string guid))
            {
                guidMap.Remove(_name);
                RemoveDataFromGUID(guid);
            }
        }

        private void RemoveDataFromGUID(string _guid)
        {
            if (string.IsNullOrEmpty(_guid)) return;

            if (blackboard.ContainsKey(_guid))
                blackboard.Remove(_guid);
        }

        public void Clean()
        {
            foreach (var item in guidMap.ToArray())
            {
                if (!blackboard.ContainsKey(item.Value))
                    guidMap.Remove(item.Key);
            }
        }
        #endregion
    }

    public interface IBlackboardPropertyGUID : IBlackboardProperty
    {
        string GUID { get; }
    }

    [Serializable]
    public class BlackboardPropertyGUID<T> : BlackboardProperty<T>, IBlackboardPropertyGUID
    {
        [SerializeField]
        readonly string guid;
        public string GUID { get { return guid; } }

        public BlackboardPropertyGUID()
        {
            guid = Guid.NewGuid().ToString();
        }
    }
}
