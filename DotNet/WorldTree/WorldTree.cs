using System;
using System.Collections.Generic;

namespace CZToolKit
{
    public class WorldTree : IDisposable
    {
        private Queue<int> fixedUpdateEntitiesQueue;
        private Queue<int> updateEntitiesQueue;
        private Queue<int> lateUpdateEntitiesQueue;
        private Dictionary<int, Node> entities;
        private int lastInstanceId;
        private Scene root;

        public Scene Root
        {
            get { return root; }
        }

        public WorldTree()
        {
            this.fixedUpdateEntitiesQueue = new Queue<int>(256);
            this.updateEntitiesQueue = new Queue<int>(256);
            this.lateUpdateEntitiesQueue = new Queue<int>(256);
            this.entities = new Dictionary<int, Node>(256);
            this.root = Scene.NewRootScene("Root", this);
        }

        public void Dispose()
        {
            root.Dispose();
            fixedUpdateEntitiesQueue.Clear();
            updateEntitiesQueue.Clear();
            lateUpdateEntitiesQueue.Clear();
            entities.Clear();
        }

        public int GenerateInstanceId()
        {
            return ++lastInstanceId;
        }

        public void Add(Node entity)
        {
            this.entities.Add(entity.InstanceId, entity);
            var entityType = entity.GetType();
            if (WorldTreeSystems.GetSystems(entityType, typeof(IFixedUpdateSystem)) != null)
                fixedUpdateEntitiesQueue.Enqueue(entity.InstanceId);
            if (WorldTreeSystems.GetSystems(entityType, typeof(IUpdateSystem)) != null)
                updateEntitiesQueue.Enqueue(entity.InstanceId);
            if (WorldTreeSystems.GetSystems(entityType, typeof(ILateUpdateSystem)) != null)
                lateUpdateEntitiesQueue.Enqueue(entity.InstanceId);
        }

        public void Remove(int instanceId)
        {
            this.entities.Remove(instanceId);
        }

        public Node Get(int instanceId)
        {
            this.entities.TryGetValue(instanceId, out var component);
            return component;
        }

        public void FixedUpdate()
        {
            WorldTreeSystems.FixedUpdate(this, fixedUpdateEntitiesQueue);
        }

        public void Update()
        {
            WorldTreeSystems.Update(this, updateEntitiesQueue);
        }

        public void LateUpdate()
        {
            WorldTreeSystems.LateUpdate(this, lateUpdateEntitiesQueue);
        }
    }
}