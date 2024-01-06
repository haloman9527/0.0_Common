using System.Collections.Generic;
using CZToolKit.Singletons;
using UnityEngine;

namespace CZToolKit.ET
{
    public class Root : Singleton<Root>, ISingletonAwake, ISingletonDestory, ISingletonUpdate, ISingletonLateUpdate
    {
        private Queue<int> entitiesQueue;
        private Dictionary<int, Entity> entities;
        private Scene scene;
        private int lastInstanceId;

        public Scene Scene
        {
            get { return scene; }
        }
        
        public Queue<int> EntitiesQueue
        {
            get { return entitiesQueue; }
        }

        public void Awake()
        {
            this.entitiesQueue = new Queue<int>(256);
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
            this.entitiesQueue.Enqueue(entity.InstanceId);
        }

        public void Remove(int instanceId)
        {
            this.entities.Remove(instanceId);
        }

        public Entity Get(int instanceId)
        {
            this.entities.TryGetValue(instanceId, out var component);
            return component;
        }

        public void Update()
        {
            Systems.Update(entitiesQueue);
        }

        public void LateUpdate()
        {
            Systems.LateUpdate(entitiesQueue);
        }
    }
}