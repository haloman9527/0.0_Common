using System;

namespace Moyo
{
    public interface ILogger
    {
        void Trace(string msg);
        void Warning(string msg);
        void Info(string msg);
        void Debug(string msg);
        void Error(string msg);
        void Error(Exception e);
    }
}