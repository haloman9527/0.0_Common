using System;

namespace CZToolKit
{
    public interface IUpdateSystem : ISystem
    {
        
    }

    public abstract class UpdateSystem<T> : TriggerSystem<T, IUpdateSystem> where T : Node
    {
        public override sealed void Execute(Node o)
        {
            Update((T)o);
        }

        protected abstract void Update(T o);
    }
}