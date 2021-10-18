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
using System.Collections.Generic;
using UnityEngine;

namespace CZToolKit.Core.Blackboards
{
    public interface IBlackboard
    {
        bool ContainsName(string _name);

        bool SetData(string _name, ICZType _data);

        bool TryGetData(string _name, out ICZType _data);

        bool TryGetValue<T>(string _name, out T _value);

        void SetValue<T>(string _name, T _value);

        bool RemoveData(string _name);

        bool Rename(string _oldName, string _newName);
    }

    public interface IReadOnlyBlackboard
    {
        IReadOnlyDictionary<string, ICZType> DataMap { get; }

        bool ContainsName(string _name);

        bool TryGetData(string _name, out ICZType _data);

        bool TryGetValue<T>(string _name, out T _value);
    }

    [Serializable]
    public class CZDataDictionary : Dictionary<string, ICZType> { }

    [Serializable]
    public class CZBlackboard : IReadOnlyBlackboard, IBlackboard
    {
        /// <summary> key是guid </summary>
        [SerializeField]
        CZDataDictionary dataMap = new CZDataDictionary();

        public IReadOnlyDictionary<string, ICZType> DataMap { get { return dataMap; } }

        public bool ContainsName(string _name)
        {
            return dataMap.ContainsKey(_name);
        }

        public bool SetData(string _name, ICZType _data)
        {
            if (ContainsName(_name))
                return false;
            dataMap[_name] = _data;
            return true;
        }

        public bool TryGetData(string _name, out ICZType _data)
        {
            if (dataMap.TryGetValue(_name, out _data) || _data == null)
            {
                dataMap.Remove(_name);
                return false;
            }
            return true;
        }

        public bool TryGetValue<T>(string _name, out T _value)
        {
            _value = default;
            if (!dataMap.TryGetValue(_name, out ICZType data)) return false;
            if (data is CZType<T> tData)
            {
                _value = tData.Value;
                return true;
            }
            return false;
        }

        public void SetValue<T>(string _name, T _value)
        {
            if (!dataMap.TryGetValue(_name, out ICZType data))
                dataMap[_name] = data = new CZType<T>(_value);
            else if (data is CZType<T> tData)
                tData.Value = _value;
        }

        public bool RemoveData(string _name)
        {
            if (dataMap.ContainsKey(_name))
                dataMap.Remove(_name);
            return true;
        }

        public bool Rename(string _oldName, string _newName)
        {
            if (string.IsNullOrEmpty(_oldName) || string.IsNullOrEmpty(_newName)) return false;
            if (!dataMap.TryGetValue(_oldName, out ICZType data)) return false;
            if (dataMap.ContainsKey(_newName)) return false;

            dataMap.Remove(_oldName);
            dataMap[_newName] = data;
            return true;
        }
    }
}
