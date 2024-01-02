#if UNITY_5_3_OR_NEWER
using System;
using System.Reflection;

namespace CZToolKit
{
    public class ULogger : ILogger, UnityEngine.ILogger
    {
        private UnityEngine.ILogger defaultLogger;

        public ULogger()
        {
            var defaultLoggerField = typeof(UnityEngine.Debug).GetField("s_DefaultLogger", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            var loggerField = typeof(UnityEngine.Debug).GetField("s_Logger", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            defaultLogger = defaultLoggerField.GetValue(null) as UnityEngine.ILogger;

            this.logHandler = defaultLogger.logHandler;
            this.logEnabled = true;
            this.filterLogType = UnityEngine.LogType.Log;

            loggerField.SetValue(null, this);
        }

        #region Unity ILogger

        public UnityEngine.ILogHandler logHandler { get; set; }
        public bool logEnabled { get; set; }
        public UnityEngine.LogType filterLogType { get; set; }

        public void LogFormat(UnityEngine.LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            defaultLogger.LogFormat(logType, context, format, args);
        }

        public void LogException(Exception exception, UnityEngine.Object context)
        {
            defaultLogger.LogException(exception, context);
        }

        public bool IsLogTypeAllowed(UnityEngine.LogType logType)
        {
            return defaultLogger.IsLogTypeAllowed(logType);
        }

        public void Log(UnityEngine.LogType logType, object message)
        {
            defaultLogger.Log(logType, message);
        }

        public void Log(UnityEngine.LogType logType, object message, UnityEngine.Object context)
        {
            defaultLogger.Log(logType, message, context);
        }

        public void Log(UnityEngine.LogType logType, string tag, object message)
        {
            defaultLogger.Log(logType, tag, message);
        }

        public void Log(UnityEngine.LogType logType, string tag, object message, UnityEngine.Object context)
        {
            defaultLogger.Log(logType, tag, message, context);
        }

        public void Log(object message)
        {
            defaultLogger.Log(message);
        }

        public void Log(string tag, object message)
        {
            defaultLogger.Log(tag, message);
        }

        public void Log(string tag, object message, UnityEngine.Object context)
        {
            defaultLogger.Log(tag, message, context);
        }

        public void LogWarning(string tag, object message)
        {
            defaultLogger.LogWarning(tag, message);
        }

        public void LogWarning(string tag, object message, UnityEngine.Object context)
        {
            defaultLogger.LogWarning(tag, message, context);
        }

        public void LogError(string tag, object message)
        {
            defaultLogger.LogError(tag, message);
        }

        public void LogError(string tag, object message, UnityEngine.Object context)
        {
            defaultLogger.LogError(tag, message, context);
        }

        public void LogFormat(UnityEngine.LogType logType, string format, params object[] args)
        {
            defaultLogger.LogFormat(logType, format, args);
        }

        public void LogException(Exception exception)
        {
            defaultLogger.LogException(exception);
        }

        #endregion


        #region

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

        #endregion
    }
}
#endif