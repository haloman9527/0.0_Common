using System;

namespace Moyo
{
    public interface ISystem
    {
        Type NodeType();

        Type SystemType();
    }

    public interface ISystem_E : ISystem
    {
        void Execute(Node n);
    }

    public interface ISystem_EA<TArg> : ISystem
    {
        void Execute(Node n, TArg arg);
    }
}