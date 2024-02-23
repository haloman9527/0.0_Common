using System;
using System.Collections.Generic;

namespace CZToolKit
{
    public class Root : Singleton<Root>, ISingletonAwake, ISingletonDestory, ISingletonFixedUpdate, ISingletonUpdate, ISingletonLateUpdate
    {
        private static Dictionary<Type, HashSet<Type>> s_CustomMarkTypes = new Dictionary<Type, HashSet<Type>>();

        private Queue<int> fixedUpdateEntitiesQueue;
        private Queue<int> updateEntitiesQueue;
        private Queue<int> lateUpdateEntitiesQueue;
        private Dictionary<int, Entity> entities;
        private Scene scene;
        private int lastInstanceId;

        public Scene Scene
        {
            get { return scene; }
        }

        public void Awake()
        {
            this.fixedUpdateEntitiesQueue = new Queue<int>(256);
            this.updateEntitiesQueue = new Queue<int>(256);
            this.lateUpdateEntitiesQueue = new Queue<int>(256);
            this.entities = new Dictionary<int, Entity>(256);
            this.scene = new Scene("Root", null);
        }

        public void Destroy()
        {
            scene.Dispose();
        }

        public int GenerateInstanceId()
        {
            return ++lastInstanceId;
        }

        public void Add(Entity entity)
        {
            this.entities.Add(entity.InstanceId, entity);
            var entityType = entity.GetType();
            if (Systems.GetSystems(entityType, typeof(IFixedUpdateSystem)) != null)
                fixedUpdateEntitiesQueue.Enqueue(entity.InstanceId);
            if (Systems.GetSystems(entityType, typeof(IUpdateSystem)) != null)
                updateEntitiesQueue.Enqueue(entity.InstanceId);
            if (Systems.GetSystems(entityType, typeof(ILateUpdateSystem)) != null)
                lateUpdateEntitiesQueue.Enqueue(entity.InstanceId);
        }

        public void Remove(int instanceId)
        {
            if (this.entities.TryGetValue(instanceId, out var entity))
            {
                
            }
            this.entities.Remove(instanceId);
        }

        public Entity Get(int instanceId)
        {
            this.entities.TryGetValue(instanceId, out var component);
            return component;
        }

        public void FixedUpdate()
        {
            Systems.FixedUpdate(fixedUpdateEntitiesQueue);
        }

        public void Update()
        {
            Systems.Update(updateEntitiesQueue);
        }

        public void LateUpdate()
        {
            Systems.LateUpdate(lateUpdateEntitiesQueue);
        }
    }
}