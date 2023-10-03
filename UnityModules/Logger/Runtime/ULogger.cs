#if UNITY_5_3_OR_NEWER
using System;

namespace CZToolKit
{
    public class ULogger : ILogger
    {
        public void Trace(string msg)
        {
            UnityEngine.Debug.Log(msg);
        }

        public void Debug(string msg)
        {
            UnityEngine.Debug.Log(msg);
        }

        public void Info(string msg)
        {
            UnityEngine.Debug.Log(msg);
        }

        public void Warning(string msg)
        {
            UnityEngine.Debug.LogWarning(msg);
        }

        public void Error(string msg)
        {
            UnityEngine.Debug.LogError(msg);
        }

        public void Error(Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }

        public void Trace(string msg, params object[] args)
        {
            UnityEngine.Debug.LogFormat(msg, args);
        }

        public void Warning(string msg, params object[] args)
        {
            UnityEngine.Debug.LogWarningFormat(msg, args);
        }

        public void Info(string msg, params object[] args)
        {
            UnityEngine.Debug.LogFormat(msg, args);
        }

        public void Debug(string msg, params object[] args)
        {
            UnityEngine.Debug.LogFormat(msg, args);
        }

        public void Error(string msg, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(msg, args);
        }
    }
}
#endif