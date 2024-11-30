#if UNITY_5_3_OR_NEWER
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Moyo
{
    public partial class ULogger : ILogger, UnityEngine.ILogger
    {
        private UnityEngine.ILogger defaultLogger;

        public ULogger()
        {
            var defaultLoggerField = typeof(UnityEngine.Debug).GetField("s_DefaultLogger", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            defaultLogger = defaultLoggerField.GetValue(null) as UnityEngine.ILogger;
            var loggerField = typeof(UnityEngine.Debug).GetField("s_Logger", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            loggerField.SetValue(null, this);
        }

        public ULogger(LogType filterLogType) : this()
        {
            this.filterLogType = filterLogType;
        }

        #region Unity ILogger

        public UnityEngine.ILogHandler logHandler
        {
            get { return defaultLogger.logHandler; }
            set { defaultLogger.logHandler = value; }
        }

        public bool logEnabled
        {
            get { return defaultLogger.logEnabled; }
            set { defaultLogger.logEnabled = value; }
        }

        public UnityEngine.LogType filterLogType
        {
            get { return defaultLogger.filterLogType; }
            set { defaultLogger.filterLogType = value; }
        }

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

    public partial class ULogger
    {
#if UNITY_EDITOR
        /// <summary>
        /// 最大匹配检索深度
        /// </summary>
        private const int MaxRegexMatch = 20;

        // 处理asset打开的callback函数
        // [UnityEditor.Callbacks.OnOpenAssetAttribute(0)]
        static bool OnOpenAsset(int instance, int line)
        {
            // 自定义函数，用来获取stacktrace
            string stack_trace = GetStackTrace();

            // 通过stacktrace来判断是否是自定义Log
            if (!string.IsNullOrEmpty(stack_trace))
            {
                //匹配所有Log行
                // 先匹配in格式，异常和异步
                var matches = Regex.Match(stack_trace, @"(?<=at )(.*cs):(\d+)", RegexOptions.IgnoreCase);
                
                while (matches.Success)
                {
                    if (matches.Groups[1].Value.EndsWith("ULogger.cs") || matches.Groups[1].Value.EndsWith("Log.cs"))
                    {
                        matches = matches.NextMatch();
                    }
                    else
                    {
                        break;
                    }
                }

                if (matches.Success)
                {
                    var scriptPath = matches.Groups[1].Value;
                    var lines = int.Parse(matches.Groups[2].Value);
                    UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(scriptPath, lines);
                    return true;
                }
            }

            return false;
        }

        static string GetStackTrace()
        {
            // 找到UnityEditor.EditorWindow的assembly
            var assembly_unity_editor = Assembly.GetAssembly(typeof(UnityEditor.EditorWindow));
            if (assembly_unity_editor == null) return null;

            // 找到类UnityEditor.ConsoleWindow
            var type_console_window = assembly_unity_editor.GetType("UnityEditor.ConsoleWindow");
            if (type_console_window == null) return null;
            // 找到UnityEditor.ConsoleWindow中的成员ms_ConsoleWindow
            var field_console_window = type_console_window.GetField("ms_ConsoleWindow",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            if (field_console_window == null) return null;
            // 获取ms_ConsoleWindow的值
            var instance_console_window = field_console_window.GetValue(null);
            if (instance_console_window == null) return null;

            // 如果console窗口时焦点窗口的话，获取stacktrace
            if ((object)UnityEditor.EditorWindow.focusedWindow == instance_console_window)
            {
                // 通过assembly获取类ListViewState
                var type_list_view_state = assembly_unity_editor.GetType("UnityEditor.ListViewState");
                if (type_list_view_state == null) return null;

                // 找到类UnityEditor.ConsoleWindow中的成员m_ListView
                var field_list_view = type_console_window.GetField("m_ListView",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (field_list_view == null) return null;

                // 获取m_ListView的值
                var value_list_view = field_list_view.GetValue(instance_console_window);
                if (value_list_view == null) return null;

                // 找到类UnityEditor.ConsoleWindow中的成员m_ActiveText
                var field_active_text = type_console_window.GetField("m_ActiveText",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (field_active_text == null) return null;

                // 获得m_ActiveText的值，就是我们需要的stacktrace
                string value_active_text = field_active_text.GetValue(instance_console_window).ToString();
                return value_active_text;
            }

            return null;
        }
#endif
    }
}
#endif