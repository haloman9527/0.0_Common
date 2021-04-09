using CZToolKit.Core.Blackboards;
using System.Collections.Generic;
using ICZType = CZToolKit.Core.Blackboards.ICZType;

#if UNITY_EDITOR
using UnityEngine;
#endif

public static class CZBlackboardExtension
{
    public static bool ContainsKey<T>(this Dictionary<string, ICZType> _self, string _name)
    {
        if (_self.TryGetValue(_name, out ICZType property))
        {
            if (property.GetType() is CZType<T>)
                return true;
        }
        return false;
    }

    public static bool TryGetData<T>(this Dictionary<string, ICZType> _self, string _name, out T _data, T _fallback = default)
    {
        if (_self.TryGetValue(_name, out ICZType data))
        {
            if (data is CZType<T> tData)
            {
                _data = tData.Value;
                return true;
            }
        }
        _data = _fallback;
        return false;
    }

    public static void SaveData<T>(this Dictionary<string, ICZType> _self, string _name, T _data)
    {
        if (_self.TryGetValue(_name, out ICZType property))
        {
            if (property is CZType<T> tProperty)
                tProperty.Value = _data;
        }
        else
        {
            CZType<T> tProperty = new CZType<T>(_data);
            _self[_name] = tProperty;
        }
    }

    public static void RemoveData<C>(this Dictionary<string, ICZType> _self, string _name) where C : ICZType
    {
        if (string.IsNullOrEmpty(_name)) return;

        if (_self.ContainsKey(_name))
            _self.Remove(_name);
    }

    public static bool Rename(this Dictionary<string, ICZType> _self, string _oldName, string _newName)
    {
        if (!_self.ContainsKey(_oldName))
        {
#if UNITY_EDITOR
            Debug.LogError($"{_oldName}不被包含在黑板数据内");
#endif
            return false;
        }
        if (string.IsNullOrEmpty(_newName)) return false;
        if (_self.ContainsKey(_newName))
        {
#if UNITY_EDITOR
            Debug.LogError($"黑板内已存在同名数据{_newName}");
#endif
            return false;
        }

        _self[_newName] = _self[_oldName];
        _self.Remove(_oldName);
        return true;
    }
}
