﻿using System;

namespace Moyo
{
    public interface IAddComponentSystem : ISystem
    {
        void Execute(Node o, Node c);
    }

    public abstract class AddComponentSystem<T> : IAddComponentSystem where T : Node
    {
        public Type NodeType()
        {
            return TypeCache<T>.TYPE;
        }

        public Type SystemType()
        {
            return typeof(IAddComponentSystem);
        }

        public void Execute(Node o, Node c)
        {
            AddComponent((T)o, c);
        }

        protected abstract void AddComponent(T self, Node component);
    }
}