
namespace CZToolKit
{
    public interface IConfigManager
    {
        bool HasConfig(string key);

        bool HasBool(string key);

        bool HasInt(string key);

        bool HasFloat(string key);

        bool HasString(string key);
        
        bool GetBool(string key);
        
        bool GetBool(string key, bool defaultValue);

        int GetInt(string key);

        int GetInt(string key, int defaultValue);

        float GetFloat(string key);

        float GetFloat(string key, float defaultValue);

        string GetString(string key);

        string GetString(string key, string defaultValue);

        void SetBool(string key, bool value);

        void SetInt(string key, int value);

        void SetFloat(string key, float value);

        void SetString(string key, string value);

        void RemoveConfig(string key);

        void RemoveBool(string key);

        void RemoveInt(string key);

        void RemoveFloat(string key);

        void RemoveString(string key);
        
        void RemoveAllConfigs();
    }
}