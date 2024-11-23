using System;

namespace Jiange
{
    public interface ISystem
    {
        Type NodeType();

        Type SystemType();
    }

    public interface ITriggerSystem : ISystem
    {
        void Execute(Node n);
    }

    public abstract class TriggerSystem<N, S> : ITriggerSystem where N : Node where S : ISystem
    {
        public Type NodeType()
        {
            return typeof(N);
        }

        public Type SystemType()
        {
            return typeof(S);
        }

        public abstract void Execute(Node n);
    }
}