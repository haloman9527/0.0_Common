using System;
using System.Collections.Generic;

namespace Atom
{
    public class World : IDisposable
    {
        private Dictionary<int, Node> nodes;
        private Dictionary<Type, Queue<NodeRef<Node>>> systemNodes;
        private int lastInstanceId;
        private Scene root;

        public Scene Root => root;

        public World()
        {
            this.nodes = new Dictionary<int, Node>(256);
            this.systemNodes = new Dictionary<Type, Queue<NodeRef<Node>>>();
            this.root = new Scene("Root", this);
        }

        public void Dispose()
        {
            lock (this)
            {
                this.root.Dispose();
                this.nodes.Clear();
                this.systemNodes.Clear();
                this.root = null;
                lastInstanceId = 0;
            }
        }

        public int GenerateInstanceId()
        {
            return ++lastInstanceId;
        }

        public Queue<NodeRef<Node>> GetQueue(Type type)
        {
            if (!this.systemNodes.TryGetValue(type, out var queue))
            {
                queue = new Queue<NodeRef<Node>>();
                this.systemNodes.Add(type, queue);
            }

            return queue;
        }

        public void Register(Node n)
        {
            var nodeRef = (NodeRef<Node>)n;
            this.nodes.Add(n.InstanceId, nodeRef);

            var nodeType = n.GetType();
            var oneTypeSystems = Systems.GetOneTypeSystems(nodeType);
            if (oneTypeSystems == null)
            {
                return;
            }

            foreach (Type queueType in oneTypeSystems.nodeOriginSystems.Keys)
            {
                var queue = this.GetQueue(queueType);
                queue.Enqueue(n);
            }
        }

        public void Unregister(int instanceId)
        {
            this.nodes.Remove(instanceId);
        }

        public Node Get(int instanceId)
        {
            this.nodes.TryGetValue(instanceId, out var component);
            return component;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"> SystemType </typeparam>
        public void Publish<T>() where T : ISystem_E
        {
            var queue = GetQueue(TypeCache<T>.TYPE);
            if (queue == null)
            {
                return;
            }

            int count = queue.Count;
            while (count-- > 0)
            {
                var component = (Node)queue.Dequeue();
                if (component == null)
                {
                    continue;
                }

                if (component.IsDisposed)
                {
                    continue;
                }

                queue.Enqueue(component);

                var systems = Systems.GetSystems(component.GetType(), TypeCache<T>.TYPE);

                for (int i = 0; i < systems.Count; i++)
                {
                    try
                    {
                        ((ISystem_E)systems[i]).Execute(component);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
        }
    }

    public struct NodeRef<T> where T : Node
    {
        private readonly long instanceId;
        private T node;

        private NodeRef(T t)
        {
            if (t == null)
            {
                this.instanceId = 0;
                this.node = null;
            }
            else
            {
                this.instanceId = t.InstanceId;
                this.node = t;
            }
        }

        private T UnWrap
        {
            get
            {
                if (this.node == null)
                {
                    return null;
                }

                if (this.node.InstanceId != this.instanceId)
                {
                    // 这里instanceId变化了，设置为null，解除引用，好让runtime去gc
                    this.node = null;
                }

                return this.node;
            }
        }

        public static implicit operator NodeRef<T>(T t)
        {
            return new NodeRef<T>(t);
        }

        public static implicit operator T(NodeRef<T> v)
        {
            return v.UnWrap;
        }
    }
}