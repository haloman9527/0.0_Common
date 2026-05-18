using System;
using System.Collections.Generic;
using UnityEngine;

namespace Atom
{
    public class PrefsVariable<T>
    {
        private string m_Key;
        private T      m_Value;
        private bool   m_Initialized;
        public event Action<T> OnValueChanged;

        public PrefsVariable(string key, T defaultValue = default(T))
        {
            this.m_Key = key;
            this.m_Value = defaultValue;
        }

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
                    v!.m_Value = PlayerPrefs.GetInt(m_Key, v.m_Value);
                    break;
                }
                case PrefsVariable<float> v:
                {
                    v!.m_Value = PlayerPrefs.GetFloat(m_Key, v.m_Value);
                    break;
                }
                case PrefsVariable<bool> v:
                {
                    v!.m_Value = PlayerPrefs.GetInt(m_Key, v.m_Value ? 1 : 0) != 0;
                    break;
                }
                case PrefsVariable<string> v:
                {
                    v!.m_Value = PlayerPrefs.GetString(m_Key, v.m_Value);
                    break;
                }
                default:
                {
                    var valueType = typeof(T);
                    if (valueType.IsEnum)
                    {
                        int intValue = PlayerPrefs.GetInt(m_Key, (int)(object)m_Value);
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
                    PlayerPrefs.SetInt(m_Key, v.m_Value);
                    break;
                }
                case PrefsVariable<float> v:
                {
                    PlayerPrefs.SetFloat(m_Key, v.m_Value);
                    break;
                }
                case PrefsVariable<bool> v:
                {
                    PlayerPrefs.SetInt(m_Key, v.m_Value ? 1 : 0);
                    break;
                }
                case PrefsVariable<string> v:
                {
                    PlayerPrefs.SetString(m_Key, v.m_Value);
                    break;
                }
                default:
                {
                    var valueType = typeof(T);
                    if (valueType.IsEnum)
                    {
                        PlayerPrefs.SetInt(m_Key, (int)(object)m_Value);
                    }
                    else
                    {
                        throw new ArgumentException($"variable type {typeof(T).Name} is not supported, Only [int|float|bool|string] types are supported.");
                    }

                    break;
                }
            }
        }
    }
}