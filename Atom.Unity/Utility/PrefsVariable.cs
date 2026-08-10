using System;
using System.Collections.Generic;

namespace Atom.Unity
{
    public abstract class PrefsVariable<T>
    {
        private string         m_Key;
        private T              m_Value;
        private bool           m_Initialized;
        public event Action<T> OnValueChanged;

        public PrefsVariable(string key, T defaultValue = default(T))
        {
            this.m_Key = key;
            this.m_Value = defaultValue;
            if (string.IsNullOrEmpty(m_Key))
                throw new ArgumentNullException($"{nameof(key)} cannot be null or empty");
        }
        
        public string Key => m_Key;

        public T Value
        {
            get
            {
                TryInitialize();
                return m_Value;
            }
            set
            {
                TryInitialize();
                if (EqualityComparer<T>.Default.Equals(this.m_Value, value))
                {
                    return;
                }

                this.m_Value = value;
                this.SaveValue();
                this.OnValueChanged?.Invoke(this.m_Value);
            }
        }

        private void TryInitialize()
        {
            if (m_Initialized)
                return;

            m_Initialized = true;
            LoadValue();
        }

        private void LoadValue()
        {
            switch (this)
            {
                case PrefsVariable<int> v:
                {
                    v!.m_Value = ReadIntValue(m_Key, v.m_Value);
                    break;
                }
                case PrefsVariable<float> v:
                {
                    v!.m_Value = ReadFloatValue(m_Key, v.m_Value);
                    break;
                }
                case PrefsVariable<bool> v:
                {
                    v!.m_Value = ReadBoolValue(m_Key, v.m_Value);
                    break;
                }
                case PrefsVariable<string> v:
                {
                    v!.m_Value = ReadStringValue(m_Key, v.m_Value);
                    break;
                }
                default:
                {
                    var valueType = typeof(T);
                    if (valueType.IsEnum)
                    {
                        int intValue = ReadIntValue(m_Key, (int)(object)m_Value);
                        m_Value = (T)Enum.ToObject(valueType, intValue);
                    }
                    else
                    {
                        throw new ArgumentException($"variable type {typeof(T).Name} is not supported, Only [int|float|bool|string] types are supported.");
                    }

                    break;
                }
            }
        }

        private void SaveValue()
        {
            switch (this)
            {
                case PrefsVariable<int> v:
                {
                    SaveIntValue(m_Key, v.Value);
                    break;
                }
                case PrefsVariable<float> v:
                {
                    SaveFloatValue(m_Key, v.Value);
                    break;
                }
                case PrefsVariable<bool> v:
                {
                    SaveBoolValue(m_Key, v.Value);
                    break;
                }
                case PrefsVariable<string> v:
                {
                    SaveToString(m_Key, v.Value);
                    break;
                }
                default:
                {
                    var valueType = typeof(T);
                    if (valueType.IsEnum)
                    {
                        SaveIntValue(m_Key, (int)(object)m_Value);
                    }
                    else
                    {
                        throw new ArgumentException($"variable type {typeof(T).Name} is not supported, Only [int|float|bool|string] types are supported.");
                    }

                    break;
                }
            }
        }
        
        protected abstract int ReadIntValue(string key, int defaultValue = 0);
        
        protected abstract void SaveIntValue(string key, int value);
        
        protected abstract float ReadFloatValue(string key, float defaultValue = 0f);
        
        protected abstract void SaveFloatValue(string key, float value);
        
        protected abstract bool ReadBoolValue(string key, bool defaultValue = false);
        
        protected abstract void SaveBoolValue(string key, bool value);
        
        protected abstract string ReadStringValue(string key, string defaultValue = null);
        
        protected abstract void SaveToString(string key, string value);
    }
}