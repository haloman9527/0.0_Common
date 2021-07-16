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
using CZToolKit.Core.Blackboards;
using System.Collections.Generic;
using UnityEngine;

using ICZType = CZToolKit.Core.Blackboards.ICZType;

public static partial class Extension_Blackboard
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

    public static void SetData<T>(this Dictionary<string, ICZType> _self, string _name, T _data)
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

    public static void RemoveData<T>(this Dictionary<string, ICZType> _self, string _name) where T : ICZType
    {
        if (string.IsNullOrEmpty(_name)) return;

        if (_self.TryGetValue(_name, out ICZType property))
        {
            if (property is CZType<T> tProperty)
                _self.Remove(_name);
        }
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
