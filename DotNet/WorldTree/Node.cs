using System;
using System.Collections.Generic;

namespace CZToolKit
{
    [Serializable]
    public class Node : IDisposable
    {
#if UNITY_EDITOR && WORLD_TREE_PREVIEW
        protected UnityEngine.GameObject viewGO;
#endif

        [NonSerialized] private int m_instanceId;
        [NonSerialized] internal WorldTree worldTree;
        [NonSerialized] protected Scene domain;
        [NonSerialized] protected Node parent;
        [NonSerialized] protected Dictionary<int, Node> children;
        [NonSerialized] protected Dictionary<Type, Node> components;

        [NonSerialized] private HashSet<Node> childrenDB;
        [NonSerialized] private HashSet<Node> componentsDB;

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
                    WorldTree.Unregister(this.m_instanceId);
                    m_instanceId = value;
                }
                else
                {
                    m_instanceId = value;
                    WorldTree.Register(this);
                }

#if UNITY_EDITOR && WORLD_TREE_PREVIEW
                if (m_instanceId != 0)
                {
                    this.viewGO = new UnityEngine.GameObject(this.GetType().Name);
                    this.viewGO.AddComponent<WorldTreeNodePreview>().component = this;
                    this.viewGO.transform.SetParent(WorldTreePreviewRoot.Instance.transform);
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

        public WorldTree WorldTree
        {
            get { return worldTree; }
        }

        /// <summary>
        /// 其实就是Scene啦，只是为了规避和<see cref="Scene"/>的命名冲突
        /// </summary>
        public Scene Domain
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

                if (this.CheckChildren())
                {
                    foreach (var o in this.children.Values)
                    {
                        o.Domain = this.domain;
                    }
                }

                if (this.CheckComponents())
                {
                    foreach (var o in this.components.Values)
                    {
                        o.Domain = this.domain;
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

                if (parent != null && CheckComponents() && parent.componentsDB.Contains(this))
                {
                    ComponentParent = value;
                    return;
                }

                this.parent = value;
                this.parent.AddToChildren(this);

                if (this is Scene scene)
                    scene.Domain = this.parent.domain;
                else
                    this.domain = this.parent.domain;

#if UNITY_EDITOR && WORLD_TREE_PREVIEW
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

                if (this.parent is Scene scene)
                    this.domain = scene;
                else
                    this.domain = this.parent.domain;

#if UNITY_EDITOR && WORLD_TREE_PREVIEW
                if (parent.viewGO.transform.Find("---------------") == null)
                {
                    new UnityEngine.GameObject("---------------").transform.SetParent(parent.viewGO.transform, false);
                }

                this.viewGO.transform.SetParent(parent.viewGO.transform, false);
                this.viewGO.transform.SetSiblingIndex(parent.components.Count - 1);
#endif
            }
        }

        public T As<T>() where T : class
        {
            return this as T;
        }

        public Node AddChild(Node o)
        {
            if (!o.IsDisposed)
            {
                throw new Exception($"has not disposed!");
            }

            o.worldTree = this.worldTree;
            o.InstanceId = WorldTree.GenerateInstanceId();
            o.Parent = this;

            WorldTreeSystems.Awake(o);

            return o;
        }

        public T AddChild<T>() where T : Node, new()
        {
            var o = new T();

            o.worldTree = this.worldTree;
            o.InstanceId = WorldTree.GenerateInstanceId();
            o.Parent = this;

            WorldTreeSystems.Awake(o);

            return o;
        }

        public T AddChild<T, A>(A a) where T : Node, new()
        {
            var o = new T();
            o.worldTree = this.worldTree;
            o.InstanceId = WorldTree.GenerateInstanceId();
            o.Parent = this;

            WorldTreeSystems.Awake(o, a);

            return o;
        }

        public T AddChild<T, A, B>(A a, B b) where T : Node, new()
        {
            var o = new T();
            o.worldTree = this.worldTree;
            o.InstanceId = WorldTree.GenerateInstanceId();
            o.Parent = this;

            WorldTreeSystems.Awake(o, a, b);

            return o;
        }

        public T AddChild<T, A, B, C>(A a, B b, C c) where T : Node, new()
        {
            var o = new T();
            o.worldTree = this.worldTree;
            o.InstanceId = WorldTree.GenerateInstanceId();
            o.Parent = this;

            WorldTreeSystems.Awake(o, a, b, c);

            return o;
        }

        public T AddChild<T, A, B, C, D>(A a, B b, C c, D d) where T : Node, new()
        {
            var o = new T();
            o.worldTree = this.worldTree;
            o.InstanceId = WorldTree.GenerateInstanceId();
            o.Parent = this;

            WorldTreeSystems.Awake(o, a, b, c, d);

            return o;
        }

        public Node GetChild(int instanceId)
        {
            if (children == null)
            {
                return null;
            }

            if (this.children.TryGetValue(instanceId, out var o))
            {
                return o;
            }

            return null;
        }

        public void RemoveChild(int instanceId)
        {
            if (this.IsDisposed)
            {
                return;
            }

            if (!this.CheckChildren())
            {
                return;
            }

            if (!this.children.TryGetValue(instanceId, out var child))
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

        private bool CheckChildren(bool autoInit = false)
        {
            if (children != null)
            {
                return true;
            }

            if (autoInit)
            {
                this.children = new Dictionary<int, Node>();
                this.childrenDB = new HashSet<Node>();
                return true;
            }

            return false;
        }

        private void AddToChildren(Node o)
        {
            this.CheckChildren(true);
            this.children.Add(o.m_instanceId, o);
            this.childrenDB.Add(o);
        }

        private void RemoveFromChildren(Node o, int instanceId)
        {
            if (!CheckChildren())
            {
                return;
            }

            this.children.Remove(instanceId);
            this.childrenDB.Remove(o);
        }

        public void AddComponent(Node component)
        {
            if (!component.IsDisposed)
            {
                throw new Exception($"component has not disposed!");
            }

            var type = component.GetType();
            if (this.CheckComponents() && this.components.ContainsKey(type))
            {
                throw new Exception($"already has component: {type.FullName}");
            }

            component.worldTree = this.worldTree;
            component.InstanceId = WorldTree.GenerateInstanceId();
            component.ComponentParent = this;

            WorldTreeSystems.Awake(component);
            WorldTreeSystems.AddComponent(this, component);
        }

        public Node AddComponent(Type type)
        {
            if (this.CheckComponents() && this.components.ContainsKey(type))
            {
                throw new Exception($"already has component: {type.FullName}");
            }

            var component = Activator.CreateInstance(type) as Node;
            component.worldTree = this.worldTree;
            component.InstanceId = WorldTree.GenerateInstanceId();
            component.ComponentParent = this;

            WorldTreeSystems.Awake(component);
            WorldTreeSystems.AddComponent(this, component);

            return component;
        }

        public Node AddComponent<A>(Type type, A a)
        {
            if (this.CheckComponents() && this.components.ContainsKey(type))
            {
                throw new Exception($"already has component: {type.FullName}");
            }

            var component = Activator.CreateInstance(type) as Node;
            component.worldTree = this.worldTree;
            component.InstanceId = WorldTree.GenerateInstanceId();
            component.ComponentParent = this;

            WorldTreeSystems.Awake(component, a);
            WorldTreeSystems.AddComponent(this, component);

            return component;
        }

        public Node AddComponent<A, B>(Type type, A a, B b)
        {
            if (this.CheckComponents() && this.components.ContainsKey(type))
            {
                throw new Exception($"already has component: {type.FullName}");
            }

            var component = Activator.CreateInstance(type) as Node;
            component.worldTree = this.worldTree;
            component.InstanceId = WorldTree.GenerateInstanceId();
            component.ComponentParent = this;

            WorldTreeSystems.Awake(component, a, b);
            WorldTreeSystems.AddComponent(this, component);

            return component;
        }

        public Node AddComponent<A, B, C>(Type type, A a, B b, C c)
        {
            if (this.CheckComponents() && this.components.ContainsKey(type))
            {
                throw new Exception($"already has component: {type.FullName}");
            }

            var component = Activator.CreateInstance(type) as Node;
            component.worldTree = this.worldTree;
            component.InstanceId = WorldTree.GenerateInstanceId();
            component.ComponentParent = this;

            WorldTreeSystems.Awake(component, a, b, c);
            WorldTreeSystems.AddComponent(this, component);

            return component;
        }

        public Node AddComponent<A, B, C, D>(Type type, A a, B b, C c, D d)
        {
            if (this.CheckComponents() && this.components.ContainsKey(type))
            {
                throw new Exception($"already has component: {type.FullName}");
            }

            var component = Activator.CreateInstance(type) as Node;
            component.worldTree = this.worldTree;
            component.InstanceId = WorldTree.GenerateInstanceId();
            component.ComponentParent = this;

            WorldTreeSystems.Awake(component, a, b, c, d);
            WorldTreeSystems.AddComponent(this, component);

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
            if (!CheckComponents())
            {
                return null;
            }

            if (this.components.TryGetValue(type, out var component))
            {
                return component;
            }

            if (deriveMatch)
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

            if (!CheckComponents())
            {
                return;
            }

            if (!components.TryGetValue(type, out var component))
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
            if (component.IsDisposed)
            {
                return;
            }

            if (this.IsDisposed)
            {
                return;
            }

            if (!CheckComponents())
            {
                return;
            }

            if (component.parent != this)
            {
                return;
            }

            component.Dispose();
        }

        private bool CheckComponents(bool autoInit = false)
        {
            if (components != null)
            {
                return true;
            }

            if (autoInit)
            {
                this.components = new Dictionary<Type, Node>();
                this.componentsDB = new HashSet<Node>();
                return true;
            }

            return false;
        }

        private void AddToComponents(Node component)
        {
            this.CheckComponents(true);
            this.components.Add(component.GetType(), component);
            this.componentsDB.Add(component);
        }

        private void RemoveFromComponents(Node component)
        {
            if (!CheckComponents())
            {
                return;
            }

            this.components.Remove(component.GetType());
            this.componentsDB.Remove(component);
        }

        public virtual void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            WorldTreeSystems.Destroy(this);

            var instanceId = this.m_instanceId;
            this.InstanceId = 0;

            if (this.CheckChildren())
            {
                foreach (Node child in this.children.Values)
                {
                    child.Dispose();
                }

                this.children.Clear();
                this.childrenDB.Clear();
            }

            if (this.CheckComponents())
            {
                foreach (Node component in this.components.Values)
                {
                    component.Dispose();
                }

                this.components.Clear();
                this.componentsDB.Clear();
            }

            if (this.parent != null && !this.parent.IsDisposed)
            {
                this.parent.RemoveFromComponents(this);
                this.parent.RemoveFromChildren(this, instanceId);
            }

            this.parent = null;
            this.domain = null;
            this.worldTree = null;
        }
    }
}