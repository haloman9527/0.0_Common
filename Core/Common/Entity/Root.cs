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
        private Dictionary<int, Node> entities;
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
            this.entities = new Dictionary<int, Node>(256);
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

        public void Add(Node node)
        {
            this.entities.Add(node.InstanceId, node);
            var nodeType = node.GetType();
            if (Systems.GetSystems(nodeType, typeof(IFixedUpdateSystem)) != null)
                fixedUpdateEntitiesQueue.Enqueue(node.InstanceId);
            if (Systems.GetSystems(nodeType, typeof(IUpdateSystem)) != null)
                updateEntitiesQueue.Enqueue(node.InstanceId);
            if (Systems.GetSystems(nodeType, typeof(ILateUpdateSystem)) != null)
                lateUpdateEntitiesQueue.Enqueue(node.InstanceId);
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