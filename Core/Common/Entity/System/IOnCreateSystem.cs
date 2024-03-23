using System;

namespace CZToolKit
{
    public interface IOnCreateSystem : ISystem
    {
        void Execute(Node o);
    }

    public abstract class OnCreateSystem<T> : IOnCreateSystem where T : Node
    {
        public Type NodeType()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IOnCreateSystem);
        }

        public void Execute(Node o)
        {
            OnCreate((T)o);
        }

        protected abstract void OnCreate(T o);
    }
}