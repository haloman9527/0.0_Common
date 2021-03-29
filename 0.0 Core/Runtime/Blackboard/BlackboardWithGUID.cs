using System;
using System.Collections.Generic;
using UnityEngine;

namespace CZToolKit.Core.Blackboards
{
    public class BlackboardWithGUID : Blackboard
    {
        /// <summary> Name与GUID映射表 </summary>
        [SerializeField]
        protected Dictionary<string, string> guidMap = new Dictionary<string, string>();

        #region GetData
        public override bool TryGetData<T>(string _name, out T _data, T _fallback = default)
        {
            if (guidMap.TryGetValue(_name, out string guid))
            {
                if (TrygetDataFromGUID(guid, out _data, _fallback))
                    return true;
            }

            _data = _fallback;
            return false;
        }

        public bool TrygetDataFromGUID<T>(string _guid, out T _data, T _fallback = default)
        {
            if (data.TryGetValue(_guid, out IBlackboardProperty property))
            {
                if (property is BlackboardPropertyGUID<T> tProperty)
                {
                    _data = tProperty.Value;
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
            return data.ContainsKey(_guid);
        }

        public override bool Contains<T>(string _name)
        {
            if (guidMap.TryGetValue(_name, out string guid))
                return ContainsGUID<T>(guid);
            return false;
        }

        public bool ContainsGUID<T>(string _guid)
        {
            if (data.TryGetValue(_guid, out IBlackboardProperty property))
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
            {
                SetDataFromGUID(guid, _data);
            }
            else
            {
                BlackboardPropertyGUID<T> tProperty = new BlackboardPropertyGUID<T>();
                tProperty.Value = _data;
                SetData(tProperty);
                guidMap[_name] = tProperty.GUID;
            }
        }

        private void SetDataFromGUID<T>(string _guid, T _data)
        {
            if (data.TryGetValue(_guid, out IBlackboardProperty property))
            {
                if (property is BlackboardPropertyGUID<T> tProperty)
                    tProperty.Value = _data;
            }
            else
            {
                BlackboardPropertyGUID<T> tProperty = new BlackboardPropertyGUID<T>();
                tProperty.Value = _data;
                SetData(tProperty);
            }
        }

        private void SetData<T>(BlackboardPropertyGUID<T> _tProperty)
        {
            data[_tProperty.GUID] = _tProperty;
        }
        #endregion

        public bool TryGetGUID(string _name, out string _guid)
        {
            return guidMap.TryGetValue(_name, out _guid);
        }

        public override bool Rename(string _oldName, string _newName)
        {
            if (!guidMap.ContainsKey(_oldName)) { Debug.LogError($"{_oldName}不被包含在黑板数据内"); return false; }
            if (string.IsNullOrEmpty(_newName)) return false;
            if (guidMap.ContainsKey(_newName)) { Debug.LogError($"黑板内已存在同名数据{_newName}"); return false; }

            guidMap[_newName] = guidMap[_oldName];
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

            if (data.ContainsKey(_guid))
                data.Remove(_guid);
        }
        #endregion
    }

    public class BlackboardPropertyGUID<T> : BlackboardProperty<T>
    {
        string name;
        readonly string guid;

        public string Name
        {
            get { return name; }
            set { if (!string.IsNullOrEmpty(value)) name = value; }
        }
        public string GUID { get { return guid; } }

        public BlackboardPropertyGUID()
        {
            guid = Guid.NewGuid().ToString();
        }
    }
}
