using UnityEngine;

namespace Atom.Unity
{
    public class PlayerPrefsVariable<T> : PrefsVariable<T>
    {
        public PlayerPrefsVariable(string key, T defaultValue = default(T)) : base(key, defaultValue)
        {
        }

        protected override int ReadIntValue(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        protected override void SaveIntValue(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        protected override float ReadFloatValue(string key, float defaultValue = 0)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }

        protected override void SaveFloatValue(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        protected override bool ReadBoolValue(string key, bool defaultValue = false)
        {
            return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
        }

        protected override void SaveBoolValue(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }

        protected override string ReadStringValue(string key, string defaultValue = null)
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        protected override void SaveToString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }
    }
}