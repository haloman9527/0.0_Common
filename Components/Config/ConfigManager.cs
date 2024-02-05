using System;
using System.Collections.Generic;
using UnityEngine;

namespace CZToolKit
{
    public class ConfigManager : Singleton<ConfigManager>, ISingletonAwake, IConfigManager
    {
        private IResourceManager resourceManager;
        private DataNode dataNode;

        public void Awake()
        {
            dataNode = new DataNode();
        }

        public void SetResourceManager(IResourceManager resourceManager)
        {
            this.resourceManager = resourceManager;
        }

        public void Init(bool force = false)
        {
            var textAsset = resourceManager.Load<TextAsset>("");
        }

        public bool HasConfig(string key)
        {
            return dataNode.data.ContainsKey(key);
        }

        public bool HasBool(string key)
        {
            return dataNode.data.TryGetValue(key, out var configValue) && (configValue.type & DataValueType.Bool) != 0;
        }

        public bool HasInt(string key)
        {
            return dataNode.data.TryGetValue(key, out var configValue) && (configValue.type & DataValueType.Int) != 0;
        }

        public bool HasFloat(string key)
        {
            return dataNode.data.TryGetValue(key, out var configValue) && (configValue.type & DataValueType.Float) != 0;
        }

        public bool HasString(string key)
        {
            return dataNode.data.TryGetValue(key, out var configValue) && (configValue.type & DataValueType.String) != 0;
        }

        public bool GetBool(string key)
        {
            dataNode.data.TryGetValue(key, out var configValue);
            return configValue.boolValue;
        }

        public bool GetBool(string key, bool defaultValue)
        {
            if (!dataNode.data.TryGetValue(key, out var configValue))
                return defaultValue;

            return configValue.boolValue;
        }

        public int GetInt(string key)
        {
            dataNode.data.TryGetValue(key, out var configValue);
            return configValue.intValue;
        }

        public int GetInt(string key, int defaultValue)
        {
            if (!dataNode.data.TryGetValue(key, out var configValue))
                return defaultValue;

            return configValue.intValue;
        }

        public float GetFloat(string key)
        {
            dataNode.data.TryGetValue(key, out var configValue);
            return configValue.floatValue;
        }

        public float GetFloat(string key, float defaultValue)
        {
            if (!dataNode.data.TryGetValue(key, out var configValue))
                return defaultValue;

            return configValue.floatValue;
        }

        public string GetString(string key)
        {
            dataNode.data.TryGetValue(key, out var configValue);
            return configValue.stringValue;
        }

        public string GetString(string key, string defaultValue)
        {
            if (!dataNode.data.TryGetValue(key, out var configValue))
                return defaultValue;

            return configValue.stringValue;
        }

        public void SetBool(string key, bool value)
        {
            dataNode.data.TryGetValue(key, out var configValue);
            configValue.boolValue = value;
            configValue.type |= DataValueType.Bool;
            dataNode.data[key] = configValue;
        }

        public void SetInt(string key, int value)
        {
            dataNode.data.TryGetValue(key, out var configValue);
            configValue.intValue = value;
            configValue.type |= DataValueType.Int;
            dataNode.data[key] = configValue;
        }

        public void SetFloat(string key, float value)
        {
            dataNode.data.TryGetValue(key, out var configValue);
            configValue.floatValue = value;
            configValue.type |= DataValueType.Float;
            dataNode.data[key] = configValue;
        }

        public void SetString(string key, string value)
        {
            dataNode.data.TryGetValue(key, out var configValue);
            configValue.stringValue = value;
            configValue.type |= DataValueType.String;
            dataNode.data[key] = configValue;
        }

        public void RemoveConfig(string key)
        {
            if (!dataNode.data.ContainsKey(key))
                return;

            dataNode.data.Remove(key);
        }

        public void RemoveBool(string key)
        {
            if (!dataNode.data.TryGetValue(key, out var configValue))
                return;

            configValue.boolValue = default;
            configValue.type &= ~DataValueType.Bool;
            dataNode.data[key] = configValue;
        }

        public void RemoveInt(string key)
        {
            if (!dataNode.data.TryGetValue(key, out var configValue))
                return;

            configValue.intValue = default;
            configValue.type &= ~DataValueType.Int;
            dataNode.data[key] = configValue;
        }

        public void RemoveFloat(string key)
        {
            if (!dataNode.data.TryGetValue(key, out var configValue))
                return;

            configValue.floatValue = default;
            configValue.type &= ~DataValueType.Float;
            dataNode.data[key] = configValue;
        }

        public void RemoveString(string key)
        {
            if (!dataNode.data.TryGetValue(key, out var configValue))
                return;

            configValue.stringValue = default;
            configValue.type &= ~DataValueType.String;
            dataNode.data[key] = configValue;
        }

        public void RemoveAllConfigs()
        {
            dataNode.data.Clear();
            dataNode.children.Clear();
        }
    }

    [Flags]
    public enum DataValueType : byte
    {
        None = 0,
        Bool = 1 << 0,
        Int = 1 << 1,
        Float = 1 << 2,
        String = 1 << 3,
    }

    public class DataNode
    {
        public readonly Dictionary<string, DataValue> data = new Dictionary<string, DataValue>();
        public readonly Dictionary<string, DataNode> children = new Dictionary<string, DataNode>();
    }

    public struct DataValue
    {
        public DataValueType type;
        public bool boolValue;
        public int intValue;
        public float floatValue;
        public string stringValue;

        public DataValue(DataValueType type, bool boolValue = default, int intValue = default, float floatValue = default, string stringValue = default)
        {
            this.type = type;
            this.boolValue = boolValue;
            this.intValue = intValue;
            this.floatValue = floatValue;
            this.stringValue = stringValue;
        }
    }
}