#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;

namespace Atom.UnityEditors
{
    public class EditorPrefsVariable<T>
    {
        private string m_Key;
        private T      m_Value;
        private bool   m_Initialized;
        public event Action<T> OnValueChanged;

        public EditorPrefsVariable(string key, T defaultValue = default(T))
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
                case EditorPrefsVariable<int> v:
                {
                    v!.m_Value = EditorPrefs.GetInt(m_Key, v.m_Value);
                    break;
                }
                case EditorPrefsVariable<float> v:
                {
                    v!.m_Value = EditorPrefs.GetFloat(m_Key, v.m_Value);
                    break;
                }
                case EditorPrefsVariable<bool> v:
                {
                    v!.m_Value = EditorPrefs.GetBool(m_Key, v.m_Value);
                    break;
                }
                case EditorPrefsVariable<string> v:
                {
                    v!.m_Value = EditorPrefs.GetString(m_Key, v.m_Value);
                    break;
                }
                default:
                {
                    var valueType = typeof(T);
                    if (valueType.IsEnum)
                    {
                        int intValue = EditorPrefs.GetInt(m_Key, (int)(object)m_Value);
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
                case EditorPrefsVariable<int> v:
                {
                    EditorPrefs.SetInt(m_Key, v.m_Value);
                    break;
                }
                case EditorPrefsVariable<float> v:
                {
                    EditorPrefs.SetFloat(m_Key, v.m_Value);
                    break;
                }
                case EditorPrefsVariable<bool> v:
                {
                    EditorPrefs.SetBool(m_Key, v.m_Value);
                    break;
                }
                case EditorPrefsVariable<string> v:
                {
                    EditorPrefs.SetString(m_Key, v.m_Value);
                    break;
                }
                default:
                {
                    var valueType = typeof(T);
                    if (valueType.IsEnum)
                    {
                        EditorPrefs.SetInt(m_Key, (int)(object)m_Value);
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
#endif