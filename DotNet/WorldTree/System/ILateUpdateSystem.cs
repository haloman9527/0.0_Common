using System;

namespace CZToolKit
{
    public interface ILateUpdateSystem : ISystem
    {
        
    }

    public abstract class LateUpdateSystem<T> : TriggerSystem<T, ILateUpdateSystem> where T : Node
    {
        public override sealed void Execute(Node o)
        {
            LateUpdate((T)o);
        }

        protected abstract void LateUpdate(T o);
    }
}