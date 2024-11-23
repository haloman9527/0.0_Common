using System;

namespace Jiange
{
    public interface IFixedUpdateSystem : ISystem
    {
        
    }

    public abstract class FixedUpdateSystem<T> : TriggerSystem<T, IFixedUpdateSystem> where T : Node
    {
        public override sealed void Execute(Node o)
        {
            FixedUpdate((T)o);
        }

        protected abstract void FixedUpdate(T o);
    }
}