#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace Atom.Unity
{
    public static class ProjectPrefs
    {
        [Serializable]
        private class SettingsFile
        {
            public List<SettingEntry> entries = new List<SettingEntry>();
        }

        [Serializable]
        private class SettingEntry
        {
            public string key;
            public string value;
        }

        private const string RootDirectory = "UserSettings";
        private static readonly Dictionary<string, Dictionary<string, string>> s_Buckets = new Dictionary<string, Dictionary<string, string>>();

        public static bool HasConfig(string bucket)
        {
            return File.Exists(GetPath(bucket));
        }

        public static bool HasKey(string bucket, string key)
        {
            ValidateKey(key);
            return GetBucket(bucket).ContainsKey(key);
        }

        public static bool GetBool(string bucket, string key, bool defaultValue = false)
        {
            if (!TryGetValue(bucket, key, out var value))
                return defaultValue;

            if (bool.TryParse(value, out var boolValue))
                return boolValue;

            return value == "1" ? true : value == "0" ? false : defaultValue;
        }

        public static int GetInt(string bucket, string key, int defaultValue = 0)
        {
            return TryGetValue(bucket, key, out var value) && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)
                ? result
                : defaultValue;
        }

        public static float GetFloat(string bucket, string key, float defaultValue = 0f)
        {
            return TryGetValue(bucket, key, out var value) && float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result)
                ? result
                : defaultValue;
        }

        public static string GetString(string bucket, string key, string defaultValue = "")
        {
            return TryGetValue(bucket, key, out var value) ? value : defaultValue;
        }

        public static void SetBool(string bucket, string key, bool value)
        {
            SetValue(bucket, key, value ? "1" : "0");
        }

        public static void SetInt(string bucket, string key, int value)
        {
            SetValue(bucket, key, value.ToString(CultureInfo.InvariantCulture));
        }

        public static void SetFloat(string bucket, string key, float value)
        {
            SetValue(bucket, key, value.ToString("R", CultureInfo.InvariantCulture));
        }

        public static void SetString(string bucket, string key, string value)
        {
            SetValue(bucket, key, value ?? string.Empty);
        }

        private static bool TryGetValue(string bucket, string key, out string value)
        {
            ValidateKey(key);
            return GetBucket(bucket).TryGetValue(key, out value);
        }

        private static void SetValue(string bucket, string key, string value)
        {
            ValidateKey(key);
            var values = GetBucket(bucket);
            values[key] = value;
            Save(bucket, values);
        }

        private static Dictionary<string, string> GetBucket(string bucket)
        {
            ValidateBucket(bucket);
            if (s_Buckets.TryGetValue(bucket, out var values))
                return values;

            values = new Dictionary<string, string>();
            s_Buckets.Add(bucket, values);

            var path = GetPath(bucket);
            if (!File.Exists(path))
                return values;

            try
            {
                var settingsFile = JsonUtility.FromJson<SettingsFile>(File.ReadAllText(path));
                if (settingsFile?.entries == null)
                    return values;

                foreach (var entry in settingsFile.entries)
                {
                    if (!string.IsNullOrEmpty(entry?.key))
                        values[entry.key] = entry.value ?? string.Empty;
                }
            }
            catch (Exception)
            {
                values.Clear();
            }

            return values;
        }

        private static void Save(string bucket, Dictionary<string, string> values)
        {
            Directory.CreateDirectory(RootDirectory);

            var settingsFile = new SettingsFile();
            var keys = new List<string>(values.Keys);
            keys.Sort(StringComparer.Ordinal);
            foreach (var key in keys)
            {
                settingsFile.entries.Add(new SettingEntry
                {
                    key = key,
                    value = values[key]
                });
            }

            var path = GetPath(bucket);
            var temporaryPath = path + ".tmp";
            File.WriteAllText(temporaryPath, JsonUtility.ToJson(settingsFile, true), new UTF8Encoding(false));
            try
            {
                if (File.Exists(path))
                    File.Replace(temporaryPath, path, null);
                else
                    File.Move(temporaryPath, path);
            }
            catch (PlatformNotSupportedException)
            {
                File.Copy(temporaryPath, path, true);
                File.Delete(temporaryPath);
            }
        }

        private static string GetPath(string bucket)
        {
            ValidateBucket(bucket);
            return Path.Combine(RootDirectory, bucket + ".json");
        }

        private static void ValidateBucket(string bucket)
        {
            if (string.IsNullOrEmpty(bucket))
                throw new ArgumentNullException(nameof(bucket));

            if (Path.GetFileName(bucket) != bucket)
                throw new ArgumentException("Bucket 不能包含目录路径。", nameof(bucket));
        }

        private static void ValidateKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
        }
    }
}
#endif