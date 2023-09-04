using System;

namespace CZToolKit
{
    public static class Log
    {
#if UNITY_5_3_OR_NEWER
        private static ILogger logger = new ULogger();
#else
        private static ILogger logger;
#endif

        public static ILogger Logger
        {
            set { logger = value; }
        }

        public static void Debug(object msg)
        {
            logger.Debug(msg.ToString());
        }

        public static void Debug(string msg)
        {
            logger.Debug(msg);
        }

        public static void Info(string msg)
        {
            logger.Info(msg);
        }

        public static void Warning(string msg)
        {
            logger.Warning(msg);
        }

        public static void Error(string msg)
        {
            logger.Error(msg);
        }

        public static void Error(Exception e)
        {
            logger.Error(e);
        }

        public static void Trace(string msg, params object[] args)
        {
            logger.Trace(msg, args);
        }

        public static void Warning(string msg, params object[] args)
        {
            logger.Warning(msg, args);
        }

        public static void Info(string msg, params object[] args)
        {
            logger.Info(msg, args);
        }

        public static void Debug(string msg, params object[] args)
        {
            logger.Debug(msg, args);
        }

        public static void Error(string msg, params object[] args)
        {
            logger.Error(msg, args);
        }
    }
}