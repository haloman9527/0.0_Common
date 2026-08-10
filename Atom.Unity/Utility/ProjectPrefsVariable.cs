#if UNITY_EDITOR
using System;

namespace Atom.Unity
{
    public class ProjectPrefsVariable<T> : PrefsVariable<T>
    {
        private string m_Bucket;
        
        public ProjectPrefsVariable(string bucket, string key, T defaultValue = default(T)) : base(key, defaultValue)
        {
            this.m_Bucket = bucket;
            if (string.IsNullOrEmpty(m_Bucket))
                throw new ArgumentNullException($"{nameof(bucket)} cannot be null or empty");
        }

        protected override int ReadIntValue(string key, int defaultValue = 0)
        {
            return ProjectPrefs.GetInt(m_Bucket, key, defaultValue);
        }

        protected override void SaveIntValue(string key, int value)
        {
            ProjectPrefs.SetInt(m_Bucket, key, value);
        }

        protected override float ReadFloatValue(string key, float defaultValue = 0)
        {
            return ProjectPrefs.GetFloat(m_Bucket, key, defaultValue);
        }

        protected override void SaveFloatValue(string key, float value)
        {
            ProjectPrefs.SetFloat(m_Bucket, key, value);
        }

        protected override bool ReadBoolValue(string key, bool defaultValue = false)
        {
            return ProjectPrefs.GetBool(m_Bucket, key, defaultValue);
        }

        protected override void SaveBoolValue(string key, bool value)
        {
            ProjectPrefs.SetBool(m_Bucket, key, value);
        }

        protected override string ReadStringValue(string key, string defaultValue = null)
        {
            return ProjectPrefs.GetString(m_Bucket, key, defaultValue);
        }

        protected override void SaveToString(string key, string value)
        {
            ProjectPrefs.SetString(m_Bucket, key, value);
        }
    }
}
#endif