using System;
using System.Collections.Generic;

namespace CZToolKit
{
    [Serializable]
    public class Node : IDisposable
    {
#if UNITY_EDITOR && NODE_PREVIEW
        protected UnityEngine.GameObject viewGO;
#endif

        [NonSerialized] private int m_instanceId;
        [NonSerialized] protected Node domain;
        [NonSerialized] protected Node parent;
        [NonSerialized] protected Dictionary<int, Node> children;
        [NonSerialized] protected Dictionary<Type, Node> components;

        private HashSet<Node> childrenDB;
        private HashSet<Node> componentsDB;

        public int InstanceId
        {
            get { return m_instanceId; }
            protected set
            {
                if (this.m_instanceId == value)
                {
                    return;
                }

                if (value == 0)
                {
                    Root.Instance.Remove(this.m_instanceId);
                    m_instanceId = value;
                }
                else
                {
                    m_instanceId = value;
                    Root.Instance.Add(this);
                }

#if UNITY_EDITOR && NODE_PREVIEW
                if (m_instanceId != 0)
                {
                    this.viewGO = new UnityEngine.GameObject(this.GetType().Name);
                    this.viewGO.AddComponent<NodePreview>().component = this;
                    this.viewGO.transform.SetParent(NodePreviewRoot.Instance.transform);
                }
                else if (parent == null || !parent.IsDisposed)
                {
                    UnityEngine.GameObject.Destroy(viewGO);
                }
#endif
            }
        }

        public bool IsDisposed
        {
            get { return m_instanceId == 0; }
        }

        /// <summary>
        /// 其实就是Scene啦，只是为了规避和<see cref="Scene"/>的命名冲突
        /// </summary>
        public Node Domain
        {
            get { return domain; }
            protected set
            {
                if (value == null)
                {
                    throw new Exception($"domain cant set null: {this.GetType().Name}");
                }

                if (this.domain == value)
                {
                    return;
                }

                this.domain = value;

                if (this.children != null)
                {
                    foreach (Node node in this.children.Values)
                    {
                        node.Domain = this.domain;
                    }
                }

                if (this.components != null)
                {
                    foreach (Node component in this.components.Values)
                    {
                        component.Domain = this.domain;
                    }
                }
            }
        }

        public Node Parent
        {
            get { return parent; }
            set
            {
                if (value == null)
                {
                    throw new Exception($"cant set parent null: {this.GetType().Name}");
                }

                if (value == this)
                {
                    throw new Exception($"cant set parent self: {this.GetType().Name}");
                }

                if (this.parent == value)
                {
                    return;
                }

                if (this.parent != null)
                {
                    this.parent.RemoveFromChildren(this, this.m_instanceId);
                }

                if (parent != null && parent.componentsDB != null && parent.componentsDB.Contains(this))
                {
                    ComponentParent = value;
                    return;
                }

                this.parent = value;
                this.parent.AddToChildren(this);

                if (this.parent is Scene)
                    this.domain = this.parent;
                else
                    this.domain = this.parent.domain;

#if UNITY_EDITOR && NODE_PREVIEW
                if (parent.viewGO.transform.Find("---------------") == null)
                {
                    new UnityEngine.GameObject("---------------").transform.SetParent(parent.viewGO.transform, false);
                }

                this.viewGO.transform.SetParent(parent.viewGO.transform, false);
                this.viewGO.transform.SetAsLastSibling();
                if (parent.componentsDB != null && parent.componentsDB.Contains(this))
                    this.viewGO.name = this.GetType().Name;
                else
                    this.viewGO.name = this.GetType().Name + $"({this.m_instanceId})";
#endif
            }
        }

        private Node ComponentParent
        {
            set
            {
                if (value == null)
                {
                    throw new Exception($"cant set parent be null: {this.GetType().Name}");
                }

                if (value == this)
                {
                    throw new Exception($"cant set parent be self: {this.GetType().Name}");
                }

                // 严格限制parent必须要有domain,也就是说parent必须在数据树上面
                if (value.Domain == null)
                {
                    throw new Exception($"cant set parent because parent domain is null: {this.GetType().Name} {value.GetType().Name}");
                }

                if (this.parent != null) // 之前有parent
                {
                    // parent相同，不设置
                    if (this.parent == value)
                    {
                        Log.Error($"重复设置了Parent: {this.GetType().Name} parent: {this.parent.GetType().Name}");
                        return;
                    }

                    this.parent.RemoveFromChildren(this, this.m_instanceId);
                }

                this.parent = value;
                this.parent.AddToComponents(this);

                if (this.parent is Scene)
                    this.domain = this.parent;
                else
                    this.domain = this.parent.domain;

#if UNITY_EDITOR && NODE_PREVIEW
                if (parent.viewGO.transform.Find("---------------") == null)
                {
                    new UnityEngine.GameObject("---------------").transform.SetParent(parent.viewGO.transform, false);
                }

                this.viewGO.transform.SetParent(parent.viewGO.transform, false);
                this.viewGO.transform.SetSiblingIndex(parent.components.Count - 1);
#endif
            }
        }

        public Dictionary<int, Node> Children
        {
            get
            {
                if (children == null)
                {
                    children = new Dictionary<int, Node>();
                }

                return this.children;
            }
        }

        public Dictionary<Type, Node> Components
        {
            get
            {
                if (components == null)
                {
                    components = new Dictionary<Type, Node>();
                }

                return this.components;
            }
        }

        public T As<T>() where T : class
        {
            return this as T;
        }

        public Node AddChild(Node node)
        {
            if (!node.IsDisposed)
            {
                throw new Exception($"node has not disposed!");
            }

            node.InstanceId = Root.Instance.GenerateInstanceId();
            node.Parent = this;

            Systems.Awake(node);
            Systems.OnCreate(node);

            return node;
        }

        public T AddChild<T>() where T : Node, new()
        {
            var node = new T();
            node.InstanceId = Root.Instance.GenerateInstanceId();
            node.Parent = this;

            Systems.Awake(node);
            Systems.OnCreate(node);

            return node;
        }

        public T AddChild<T, A>(A a) where T : Node, new()
        {
            var node = new T();
            node.InstanceId = Root.Instance.GenerateInstanceId();
            node.Parent = this;

            Systems.Awake(node, a);
            Systems.OnCreate(node);

            return node;
        }

        public T AddChild<T, A, B>(A a, B b) where T : Node, new()
        {
            var node = new T();
            node.InstanceId = Root.Instance.GenerateInstanceId();
            node.Parent = this;

            Systems.Awake(node, a, b);
            Systems.OnCreate(node);

            return node;
        }

        public T AddChild<T, A, B, C>(A a, B b, C c) where T : Node, new()
        {
            var node = new T();
            node.InstanceId = Root.Instance.GenerateInstanceId();
            node.Parent = this;

            Systems.Awake(node, a, b, c);
            Systems.OnCreate(node);

            return node;
        }

        public T AddChild<T, A, B, C, D>(A a, B b, C c, D d) where T : Node, new()
        {
            var node = new T();
            node.InstanceId = Root.Instance.GenerateInstanceId();
            node.Parent = this;

            Systems.Awake(node, a, b, c, d);
            Systems.OnCreate(node);

            return node;
        }

        public Node GetChild(int instanceId)
        {
            if (children == null)
            {
                return null;
            }

            if (this.children.TryGetValue(instanceId, out var node))
            {
                return node;
            }

            return null;
        }

        public void RemoveChild(int instanceId)
        {
            if (this.IsDisposed)
            {
                return;
            }

            var child = this.GetChild(instanceId);
            if (child == null)
            {
                return;
            }

            child.Dispose();
        }

        public void RemoveChild(Node child)
        {
            if (this.IsDisposed)
            {
                return;
            }

            if (child == null)
            {
                return;
            }

            if (child.IsDisposed)
            {
                return;
            }

            if (child.parent != this)
            {
                return;
            }

            child.Dispose();
        }

        private void AddToChildren(Node node)
        {
            this.Children.Add(node.m_instanceId, node);

            if (childrenDB == null)
            {
                childrenDB = new HashSet<Node>();
            }

            childrenDB.Add(node);
        }

        private void RemoveFromChildren(Node node, int instanceId)
        {
            if (children == null)
            {
                return;
            }

            children.Remove(instanceId);

            if (childrenDB == null)
            {
                return;
            }

            childrenDB.Remove(node);
        }

        public void AddComponent(Node component)
        {
            if (!component.IsDisposed)
            {
                throw new Exception($"component has not disposed!");
            }

            var type = component.GetType();
            if (this.components != null && this.components.ContainsKey(type))
            {
                throw new Exception($"node already has component: {type.FullName}");
            }

            component.InstanceId = Root.Instance.GenerateInstanceId();
            component.ComponentParent = this;

            Systems.Awake(component);
            Systems.OnCreate(component);
            Systems.AddComponent(this, component);
        }

        public Node AddComponent(Type type)
        {
            if (this.components != null && this.components.ContainsKey(type))
            {
                throw new Exception($"node already has component: {type.FullName}");
            }

            var component = Activator.CreateInstance(type) as Node;
            component.InstanceId = Root.Instance.GenerateInstanceId();
            component.ComponentParent = this;

            Systems.Awake(component);
            Systems.OnCreate(component);
            Systems.AddComponent(this, component);

            return component;
        }

        public Node AddComponent<A>(Type type, A a)
        {
            if (this.components != null && this.components.ContainsKey(type))
            {
                throw new Exception($"node already has component: {type.FullName}");
            }

            var component = Activator.CreateInstance(type) as Node;
            component.InstanceId = Root.Instance.GenerateInstanceId();
            component.ComponentParent = this;

            Systems.Awake(component, a);
            Systems.OnCreate(component);
            Systems.AddComponent(this, component);

            return component;
        }

        public Node AddComponent<A, B>(Type type, A a, B b)
        {
            if (this.components != null && this.components.ContainsKey(type))
            {
                throw new Exception($"node already has component: {type.FullName}");
            }

            var component = Activator.CreateInstance(type) as Node;
            component.InstanceId = Root.Instance.GenerateInstanceId();
            component.ComponentParent = this;

            Systems.Awake(component, a, b);
            Systems.OnCreate(component);
            Systems.AddComponent(this, component);

            return component;
        }

        public Node AddComponent<A, B, C>(Type type, A a, B b, C c)
        {
            if (this.components != null && this.components.ContainsKey(type))
            {
                throw new Exception($"node already has component: {type.FullName}");
            }

            var component = Activator.CreateInstance(type) as Node;
            component.InstanceId = Root.Instance.GenerateInstanceId();
            component.ComponentParent = this;

            Systems.Awake(component, a, b, c);
            Systems.OnCreate(component);
            Systems.AddComponent(this, component);

            return component;
        }

        public Node AddComponent<A, B, C, D>(Type type, A a, B b, C c, D d)
        {
            if (this.components != null && this.components.ContainsKey(type))
            {
                throw new Exception($"node already has component: {type.FullName}");
            }

            var component = Activator.CreateInstance(type) as Node;
            component.InstanceId = Root.Instance.GenerateInstanceId();
            component.ComponentParent = this;

            Systems.Awake(component, a, b, c, d);
            Systems.OnCreate(component);
            Systems.AddComponent(this, component);

            return component;
        }

        public T AddComponent<T>() where T : Node, new()
        {
            return (T)AddComponent(typeof(T));
        }

        public T AddComponent<T, A>(A a) where T : Node, new()
        {
            return (T)AddComponent(typeof(T), a);
        }

        public T AddComponent<T, A, B>(A a, B b) where T : Node, new()
        {
            return (T)AddComponent(typeof(T), a, b);
        }

        public T AddComponent<T, A, B, C>(A a, B b, C c) where T : Node, new()
        {
            return (T)AddComponent(typeof(T), a, b, c);
        }

        public T AddComponent<T, A, B, C, D>(A a, B b, C c, D d) where T : Node, new()
        {
            return (T)AddComponent(typeof(T), a, b, c, d);
        }

        /// <summary>
        /// 获取组件，如果没有则返回null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>() where T : Node, new()
        {
            return (T)(GetComponent(typeof(T)));
        }

        /// <summary>
        /// 匹配组件，如果没有则返回null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T MatchComponent<T>()
        {
            return (T)(GetComponent(typeof(T), true) as object);
        }

        /// <summary>
        /// 获取组件，如果没有则返回null
        /// </summary>
        /// <param name="type"></param>
        /// <param name="deriveMatch"> 是否匹配派生类 </param>
        /// <returns></returns>
        public Node GetComponent(Type type, bool deriveMatch = false)
        {
            if (components == null)
            {
                return null;
            }

            if (!this.components.TryGetValue(type, out var component) && deriveMatch)
            {
                foreach (var pair in components)
                {
                    if (type.IsAssignableFrom(pair.Key))
                    {
                        component = pair.Value;
                        break;
                    }
                }
            }

            return component;
        }

        public void RemoveComponent(Type type)
        {
            if (this.IsDisposed)
            {
                return;
            }

            var component = this.GetComponent(type);
            if (component == null)
            {
                return;
            }

            component.Dispose();
        }

        public void RemoveComponent<T>() where T : Node
        {
            RemoveComponent(typeof(T));
        }

        public void RemoveComponent(Node component)
        {
            if (component == null)
            {
                return;
            }

            if (this.IsDisposed)
            {
                return;
            }

            if (component.IsDisposed)
            {
                return;
            }

            if (component.parent != this)
            {
                return;
            }

            component.Dispose();
        }

        private void AddToComponents(Node component)
        {
            this.Components.Add(component.GetType(), component);

            if (componentsDB == null)
            {
                componentsDB = new HashSet<Node>();
            }

            componentsDB.Add(component);
        }

        private void RemoveFromComponents(Node component)
        {
            if (components == null)
            {
                return;
            }

            this.components.Remove(component.GetType());

            if (componentsDB == null)
            {
                return;
            }

            componentsDB.Remove(component);
        }

        public virtual void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            Systems.Destroy(this);

            var instanceId = this.m_instanceId;
            this.InstanceId = 0;

            if (this.children != null)
            {
                foreach (Node child in this.children.Values)
                {
                    child.Dispose();
                }

                this.children.Clear();
            }

            if (this.components != null)
            {
                foreach (Node component in this.components.Values)
                {
                    component.Dispose();
                }

                this.components.Clear();
            }

            if (this.parent != null && !this.parent.IsDisposed)
            {
                this.parent.RemoveFromComponents(this);
                this.parent.RemoveFromChildren(this, instanceId);
            }

            this.parent = null;
            this.domain = null;
        }
    }
}