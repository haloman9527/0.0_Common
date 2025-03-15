﻿using System;
using System.Collections.Generic;

namespace Atom
{
    [Serializable]
    public class Node : IDisposable
    {
#if UNITY_EDITOR && WORLD_TREE_PREVIEW
        protected UnityEngine.GameObject viewGO;
#endif

        [NonSerialized] private int m_instanceId;
        [NonSerialized] internal World world;
        [NonSerialized] protected Scene scene;
        [NonSerialized] protected Node parent;
        [NonSerialized] protected Dictionary<int, Node> children;
        [NonSerialized] protected Dictionary<Type, Node> components;

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
                    World.Unregister(this.m_instanceId);
                    m_instanceId = value;
                }
                else
                {
                    m_instanceId = value;
                    World.Register(this);
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

        public World World
        {
            get { return world; }
        }

        /// <summary>
        /// 节点所在的分支
        /// </summary>
        public Scene IScene
        {
            get { return scene; }
            protected set
            {
                if (value == null)
                {
                    throw new Exception($"scene cant set null: {this.GetType().Name}");
                }

                if (this.scene == value)
                {
                    return;
                }

                this.scene = value;

                if (this.CheckChildren())
                {
                    foreach (var o in this.children.Values)
                    {
                        o.IScene = this.scene;
                    }
                }

                if (this.CheckComponents())
                {
                    foreach (var o in this.components.Values)
                    {
                        o.IScene = this.scene;
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
                    scene.IScene = this.parent.scene;
                else
                    this.scene = this.parent.scene;

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

                // 严格限制parent必须要有scene,也就是说parent必须在数据树上面
                if (value.IScene == null)
                {
                    throw new Exception($"cant set parent because parent scene is null: {this.GetType().Name} {value.GetType().Name}");
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
                    this.scene = scene;
                else
                    this.scene = this.parent.scene;

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

            o.world = this.world;
            o.InstanceId = World.GenerateInstanceId();
            o.Parent = this;

            Systems.Awake(o);

            return o;
        }

        public T AddChild<T>() where T : Node, new()
        {
            var o = new T();

            o.world = this.world;
            o.InstanceId = World.GenerateInstanceId();
            o.Parent = this;

            Systems.Awake(o);

            return o;
        }

        public T AddChild<T, A>(A a) where T : Node, new()
        {
            var o = new T();
            o.world = this.world;
            o.InstanceId = World.GenerateInstanceId();
            o.Parent = this;

            Systems.Awake(o, a);

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
                return true;
            }

            return false;
        }

        private void AddToChildren(Node o)
        {
            this.CheckChildren(true);
            this.children.Add(o.m_instanceId, o);
        }

        private void RemoveFromChildren(Node o, int instanceId)
        {
            if (!CheckChildren())
            {
                return;
            }

            this.children.Remove(instanceId);
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

            component.world = this.world;
            component.InstanceId = World.GenerateInstanceId();
            component.ComponentParent = this;

            Systems.Awake(component);
            Systems.AddComponent(this, component);
        }

        public Node AddComponent(Type type)
        {
            if (this.CheckComponents() && this.components.ContainsKey(type))
            {
                throw new Exception($"already has component: {type.FullName}");
            }

            var component = Activator.CreateInstance(type) as Node;
            component.world = this.world;
            component.InstanceId = World.GenerateInstanceId();
            component.ComponentParent = this;

            Systems.Awake(component);
            Systems.AddComponent(this, component);

            return component;
        }

        public Node AddComponent<TArg>(Type type, TArg arg)
        {
            if (this.CheckComponents() && this.components.ContainsKey(type))
            {
                throw new Exception($"already has component: {type.FullName}");
            }

            var component = Activator.CreateInstance(type) as Node;
            component.world = this.world;
            component.InstanceId = component.world.GenerateInstanceId();
            component.ComponentParent = this;

            Systems.Awake(component, arg);
            Systems.AddComponent(this, component);

            return component;
        }

        public N AddComponent<N>() where N : Node, new()
        {
            return (N)AddComponent(TypeCache<N>.TYPE, TypeCache<N>.HASH);
        }

        public N AddComponent<N, TArg>(TArg arg) where N : Node, new()
        {
            return (N)AddComponent(TypeCache<N>.TYPE, arg);
        }

        /// <summary>
        /// 获取组件，如果没有则返回null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>() where T : Node, new()
        {
            return (T)(GetComponent(TypeCache<T>.TYPE));
        }

        /// <summary>
        /// 匹配组件，如果没有则返回null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T MatchComponent<T>()
        {
            return (T)(GetComponent(TypeCache<T>.TYPE, true) as object);
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
            RemoveComponent(TypeCache<T>.TYPE);
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

            Systems.Destroy(this);

            var instanceId = this.m_instanceId;
            this.InstanceId = 0;

            if (this.CheckChildren())
            {
                foreach (Node child in this.children.Values)
                {
                    child.Dispose();
                }

                this.children.Clear();
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
            this.scene = null;
            this.world = null;
        }
    }
}