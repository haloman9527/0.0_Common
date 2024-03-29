﻿using System;

namespace CZToolKit
{
    public interface IUpdateSystem : ISystem
    {
        void Execute(Entity o);
    }

    public abstract class UpdateSystem<T> : IUpdateSystem where T : Entity
    {
        public Type NodeType()
        {
            return typeof(T);
        }

        public Type SystemType()
        {
            return typeof(IUpdateSystem);
        }

        public void Execute(Entity o)
        {
            Update((T)o);
        }

        protected abstract void Update(T o);
    }
}