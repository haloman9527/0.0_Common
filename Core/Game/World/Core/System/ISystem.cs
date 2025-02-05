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

    public interface ISystem_EA<T0> : ISystem
    {
        void Execute(Node n, T0 arg0);
    }

    public interface ISystem_EAA<T0, T1> : ISystem
    {
        void Execute(Node n, T0 arg0, T1 arg1);
    }

    public interface ISystem_EAAA<T0, T1, T2> : ISystem
    {
        void Execute(Node n, T0 arg0, T1 arg1, T2 arg2);
    }

    public interface ISystem_EAAAA<T0, T1, T2, T3> : ISystem
    {
        void Execute(Node n, T0 arg0, T1 arg1, T2 arg2, T3 arg3);
    }
}