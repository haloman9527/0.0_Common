using System;
using System.Collections.Generic;
using UnityEngine;

namespace Atom
{
    public class PlayerPrefsVariable<T>
    {
        private string key;
        private T value;
        public event Action<T> onValueChanged;

        public PlayerPrefsVariable(string key, T defaultValue = default(T))
        {
            this.key = key;
            this.value = defaultValue;
            this.LoadValue();
        }

        public T Value
        {
            get { return value; }
            set
            {
                if (EqualityComparer<T>.Default.Equals(this.value, value))
                {
                    return;
                }

                this.value = value;
                this.SaveValue();
                this.onValueChanged?.Invoke(this.value);
            }
        }

        private void LoadValue()
        {
            if (typeof(T) == typeof(int))
            {
                var v = this as PlayerPrefsVariable<int>;
                v!.value = PlayerPrefs.GetInt(key, v.value);
            }
            else if (typeof(T) == typeof(float))
            {
                var v = this as PlayerPrefsVariable<float>;
                v!.value = PlayerPrefs.GetFloat(key, v.value);
            }
            else if (typeof(T) == typeof(bool))
            {
                var v = this as PlayerPrefsVariable<bool>;
                v!.value = PlayerPrefs.GetInt(key, v.value ? 1 : 0) != 0;
            }
            else if (typeof(T) == typeof(string))
            {
                var v = this as PlayerPrefsVariable<string>;
                v!.value = PlayerPrefs.GetString(key, v.value);
            }
            else
            {
                throw new ArgumentException($"variable type {typeof(T).Name} is not supported, Only [int|float|bool|string] types are supported.");
            }
        }

        private void SaveValue()
        {
            if (typeof(T) == typeof(int))
            {
                var v = this as PlayerPrefsVariable<int>;
                PlayerPrefs.SetInt(key, v!.value);
            }
            else if (typeof(T) == typeof(float))
            {
                var v = this as PlayerPrefsVariable<float>;
                PlayerPrefs.SetFloat(key, v!.value);
            }
            else if (typeof(T) == typeof(bool))
            {
                var v = this as PlayerPrefsVariable<bool>;
                PlayerPrefs.SetInt(key, v!.value ? 1 : 0);
            }
            else if (typeof(T) == typeof(string))
            {
                var v = this as PlayerPrefsVariable<string>;
                PlayerPrefs.SetString(key, v!.value);
            }
            else
            {
                throw new ArgumentException($"variable type {typeof(T).Name} is not supported, Only [int|float|bool|string] types are supported.");
            }
        }
    }
}