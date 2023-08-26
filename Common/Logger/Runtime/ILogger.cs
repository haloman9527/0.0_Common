using System;

namespace CZToolKit.Logger
{
    public interface ILogger
    {
        void Trace(string msg);
        void Warning(string msg);
        void Info(string msg);
        void Debug(string msg);
        void Error(string msg);
        void Error(Exception e);
        void Trace(string msg, params object[] args);
        void Warning(string msg, params object[] args);
        void Info(string msg, params object[] args);
        void Debug(string msg, params object[] args);
        void Error(string msg, params object[] args);
    }
}