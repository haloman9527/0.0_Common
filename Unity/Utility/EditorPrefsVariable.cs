#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;

namespace Atom.UnityEditors
{
    public class EditorPrefsVariable<T>
    {
        private string key;
        private T value;
        public event Action<T> onValueChanged;

        public EditorPrefsVariable(string key, T defaultValue = default(T))
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
                var v = this as EditorPrefsVariable<int>;
                v!.value = EditorPrefs.GetInt(key, v.value);
            }
            else if (typeof(T) == typeof(float))
            {
                var v = this as EditorPrefsVariable<float>;
                v!.value = EditorPrefs.GetFloat(key, v.value);
            }
            else if (typeof(T) == typeof(bool))
            {
                var v = this as EditorPrefsVariable<bool>;
                v!.value = EditorPrefs.GetBool(key, v.value);
            }
            else if (typeof(T) == typeof(string))
            {
                var v = this as EditorPrefsVariable<string>;
                v!.value = EditorPrefs.GetString(key, v.value);
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
                var v = this as EditorPrefsVariable<int>;
                EditorPrefs.SetInt(key, v!.value);
            }
            else if (typeof(T) == typeof(float))
            {
                var v = this as EditorPrefsVariable<float>;
                EditorPrefs.SetFloat(key, v!.value);
            }
            else if (typeof(T) == typeof(bool))
            {
                var v = this as EditorPrefsVariable<bool>;
                EditorPrefs.SetBool(key, v!.value);
            }
            else if (typeof(T) == typeof(string))
            {
                var v = this as EditorPrefsVariable<string>;
                EditorPrefs.SetString(key, v!.value);
            }
            else
            {
                throw new ArgumentException($"variable type {typeof(T).Name} is not supported, Only [int|float|bool|string] types are supported.");
            }
        }
    }
}
#endif