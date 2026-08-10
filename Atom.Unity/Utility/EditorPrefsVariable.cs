#if UNITY_EDITOR
using UnityEditor;

namespace Atom.Unity
{
    public class EditorPrefsVariable<T> : PrefsVariable<T>
    {
        public EditorPrefsVariable(string key, T defaultValue = default(T)) : base(key, defaultValue)
        {
        }

        protected override int ReadIntValue(string key, int defaultValue = 0)
        {
            return EditorPrefs.GetInt(key, defaultValue);
        }

        protected override void SaveIntValue(string key, int value)
        {
            EditorPrefs.SetInt(key, value);
        }

        protected override float ReadFloatValue(string key, float defaultValue = 0)
        {
            return EditorPrefs.GetFloat(key, defaultValue);
        }

        protected override void SaveFloatValue(string key, float value)
        {
            EditorPrefs.SetFloat(key, value);
        }

        protected override bool ReadBoolValue(string key, bool defaultValue = false)
        {
            return EditorPrefs.GetBool(key, defaultValue);
        }

        protected override void SaveBoolValue(string key, bool value)
        {
            EditorPrefs.SetBool(key, value);
        }

        protected override string ReadStringValue(string key, string defaultValue = null)
        {
            return EditorPrefs.GetString(key, defaultValue);
        }

        protected override void SaveToString(string key, string value)
        {
            EditorPrefs.SetString(key, value);
        }
    }
}
#endif