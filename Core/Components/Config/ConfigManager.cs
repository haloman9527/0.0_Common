using System;
using System.Collections.Generic;
using CZToolKit;
using CZToolKit.Singletons;
using UnityEngine;

namespace CZToolKit
{
    public class ConfigManager : Singleton<ConfigManager>, IConfigManager
    {
        private IResourceManager resourceManager;
        private Dictionary<string, ConfigValue> configValues = new Dictionary<string, ConfigValue>();

        public void SetResourceManager(IResourceManager resourceManager)
        {
            this.resourceManager = resourceManager;
        }

        public void Init(bool force = false)
        {
            var textAsset = resourceManager.LoadSync<TextAsset>("");
        }

        public bool HasConfig(string key)
        {
            return configValues.ContainsKey(key);
        }

        public bool HasBool(string key)
        {
            return configValues.TryGetValue(key, out var configValue) && (configValue.type & ConfigValueType.Bool) != 0;
        }

        public bool HasInt(string key)
        {
            return configValues.TryGetValue(key, out var configValue) && (configValue.type & ConfigValueType.Int) != 0;
        }

        public bool HasFloat(string key)
        {
            return configValues.TryGetValue(key, out var configValue) && (configValue.type & ConfigValueType.Float) != 0;
        }

        public bool HasString(string key)
        {
            return configValues.TryGetValue(key, out var configValue) && (configValue.type & ConfigValueType.String) != 0;
        }

        public bool GetBool(string key)
        {
            configValues.TryGetValue(key, out var configValue);
            return configValue.boolValue;
        }

        public bool GetBool(string key, bool defaultValue)
        {
            if (!configValues.TryGetValue(key, out var configValue))
                return defaultValue;

            return configValue.boolValue;
        }

        public int GetInt(string key)
        {
            configValues.TryGetValue(key, out var configValue);
            return configValue.intValue;
        }

        public int GetInt(string key, int defaultValue)
        {
            if (!configValues.TryGetValue(key, out var configValue))
                return defaultValue;

            return configValue.intValue;
        }

        public float GetFloat(string key)
        {
            configValues.TryGetValue(key, out var configValue);
            return configValue.floatValue;
        }

        public float GetFloat(string key, float defaultValue)
        {
            if (!configValues.TryGetValue(key, out var configValue))
                return defaultValue;

            return configValue.floatValue;
        }

        public string GetString(string key)
        {
            configValues.TryGetValue(key, out var configValue);
            return configValue.stringValue;
        }

        public string GetString(string key, string defaultValue)
        {
            if (!configValues.TryGetValue(key, out var configValue))
                return defaultValue;

            return configValue.stringValue;
        }

        public void SetBool(string key, bool value)
        {
            configValues.TryGetValue(key, out var configValue);
            configValue.boolValue = value;
            configValue.type |= ConfigValueType.Bool;
            configValues[key] = configValue;
        }

        public void SetInt(string key, int value)
        {
            configValues.TryGetValue(key, out var configValue);
            configValue.intValue = value;
            configValue.type |= ConfigValueType.Int;
            configValues[key] = configValue;
        }

        public void SetFloat(string key, float value)
        {
            configValues.TryGetValue(key, out var configValue);
            configValue.floatValue = value;
            configValue.type |= ConfigValueType.Float;
            configValues[key] = configValue;
        }

        public void SetString(string key, string value)
        {
            configValues.TryGetValue(key, out var configValue);
            configValue.stringValue = value;
            configValue.type |= ConfigValueType.String;
            configValues[key] = configValue;
        }

        public void RemoveConfig(string key)
        {
            if (!configValues.ContainsKey(key))
                return;

            configValues.Remove(key);
        }

        public void RemoveBool(string key)
        {
            if (!configValues.TryGetValue(key, out var configValue))
                return;

            configValue.boolValue = default;
            configValue.type &= ~ConfigValueType.Bool;
            configValues[key] = configValue;
        }

        public void RemoveInt(string key)
        {
            if (!configValues.TryGetValue(key, out var configValue))
                return;

            configValue.intValue = default;
            configValue.type &= ~ConfigValueType.Int;
            configValues[key] = configValue;
        }

        public void RemoveFloat(string key)
        {
            if (!configValues.TryGetValue(key, out var configValue))
                return;

            configValue.floatValue = default;
            configValue.type &= ~ConfigValueType.Float;
            configValues[key] = configValue;
        }

        public void RemoveString(string key)
        {
            if (!configValues.TryGetValue(key, out var configValue))
                return;

            configValue.stringValue = default;
            configValue.type &= ~ConfigValueType.String;
            configValues[key] = configValue;
        }

        public void RemoveAllConfigs()
        {
            throw new System.NotImplementedException();
        }
        
        
        [Flags]
        public enum ConfigValueType : byte
        {
            None = 0,
            Bool = 1 << 0,
            Int = 1 << 1,
            Float = 1 << 2,
            String = 1 << 3,
        }
    
        public struct ConfigValue
        {
            public ConfigValueType type;
            public bool boolValue;
            public int intValue;
            public float floatValue;
            public string stringValue;

            public ConfigValue(ConfigValueType type, bool boolValue = default, int intValue = default, float floatValue = default, string stringValue = default)
            {
                this.type = type;
                this.boolValue = boolValue;
                this.intValue = intValue;
                this.floatValue = floatValue;
                this.stringValue = stringValue;
            }        
        }
    }
}