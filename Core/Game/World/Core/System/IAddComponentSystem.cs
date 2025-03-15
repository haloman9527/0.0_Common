using System;

namespace Atom
{
    public interface IAddComponentSystem : ISystem_EA<Node>
    {
        
    }

    public abstract class AddComponentSystem<T> : IAddComponentSystem where T : Node
    {
        public Type NodeType() => TypeCache<T>.TYPE;

        public Type SystemType() => typeof(IAddComponentSystem);

        public void Execute(Node o, Node c) => AddComponent((T)o, c);

        protected abstract void AddComponent(T self, Node component);
    }
}