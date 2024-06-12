
namespace CZToolKit
{
    public class ConfigManager : Singleton<ConfigManager>, IConfigManager
    {
        private IConfigManager o;

        public void Install(IConfigManager o)
        {
            this.o = o;
        }

        public void Init(bool force = false)
        {
            this.o.Init(force);
        }

        public bool HasConfig(string key)
        {
            return o.HasConfig(key);
        }

        public bool HasBool(string key)
        {
            return o.HasBool(key);
        }

        public bool HasInt(string key)
        {
            return o.HasInt(key);
        }

        public bool HasFloat(string key)
        {
            return o.HasFloat(key);
        }

        public bool HasString(string key)
        {
            return o.HasString(key);
        }

        public bool GetBool(string key)
        {
            return o.GetBool(key);
        }

        public bool GetBool(string key, bool defaultValue)
        {
            return o.GetBool(key, defaultValue);
        }

        public int GetInt(string key)
        {
            return o.GetInt(key);
        }

        public int GetInt(string key, int defaultValue)
        {
            return o.GetInt(key, defaultValue);
        }

        public float GetFloat(string key)
        {
            return o.GetFloat(key);
        }

        public float GetFloat(string key, float defaultValue)
        {
            return o.GetFloat(key, defaultValue);
        }

        public string GetString(string key)
        {
            return o.GetString(key);
        }

        public string GetString(string key, string defaultValue)
        {
            return o.GetString(key, defaultValue);
        }

        public void SetBool(string key, bool value)
        {
            o.SetBool(key, value);
        }

        public void SetInt(string key, int value)
        {
            o.SetInt(key, value);
        }

        public void SetFloat(string key, float value)
        {
            o.SetFloat(key, value);
        }

        public void SetString(string key, string value)
        {
            o.SetString(key, value);
        }

        public void Remove(string key)
        {
            o.Remove(key);
        }

        public void RemoveAll()
        {
            o.RemoveAll();
        }

        public void RemoveBool(string key)
        {
            o.RemoveBool(key);
        }

        public void RemoveInt(string key)
        {
            
            o.RemoveInt(key);
        }

        public void RemoveFloat(string key)
        {
            o.RemoveFloat(key);
        }

        public void RemoveString(string key)
        {
            o.RemoveString(key);
        }
    }
}