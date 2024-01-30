using System;
using System.Collections.Generic;

namespace CZToolKit
{
    [Serializable]
    public class Entity : IDisposable
    {
#if UNITY_EDITOR && ENTITY_PREVIEW
        protected UnityEngine.GameObject viewGO;
#endif

        [NonSerialized] private int m_instanceId;
        [NonSerialized] protected Entity domain;
        [NonSerialized] protected Entity parent;
        [NonSerialized] protected Dictionary<int, Entity> children;
        [NonSerialized] protected Dictionary<Type, Entity> components;

        private HashSet<Entity> childrenDB;
        private HashSet<Entity> componentsDB;

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

#if UNITY_EDITOR && ENTITY_PREVIEW
                if (m_instanceId != 0)
                {
                    this.viewGO = new UnityEngine.GameObject(this.GetType().Name);
                    this.viewGO.AddComponent<EntityPreview>().component = this;
                    this.viewGO.transform.SetParent(this.Parent == null ? EntityPreviewRoot.Instance.transform : this.Parent.viewGO.transform);
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
        public Entity Domain
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
                    foreach (Entity entity in this.children.Values)
                    {
                        entity.Domain = this.domain;
                    }
                }

                if (this.components != null)
                {
                    foreach (Entity component in this.components.Values)
                    {
                        component.Domain = this.domain;
                    }
                }
            }
        }

        public Entity Parent
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
                this.Domain = parent.Domain;

#if UNITY_EDITOR && ENTITY_PREVIEW
                if (parent.viewGO.transform.Find("---------------") == null)
                {
                    new UnityEngine.GameObject("---------------").transform.SetParent(parent.viewGO.transform, false);
                }

                this.viewGO.transform.SetParent(parent.viewGO.transform, false);
                this.viewGO.transform.SetAsLastSibling();
#endif
            }
        }

        private Entity ComponentParent
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
                this.domain = this.parent.domain;

#if UNITY_EDITOR && ENTITY_PREVIEW
                if (parent.viewGO.transform.Find("---------------") == null)
                {
                    new UnityEngine.GameObject("---------------").transform.SetParent(parent.viewGO.transform, false);
                }

                this.viewGO.transform.SetParent(parent.viewGO.transform, false);
                this.viewGO.transform.SetSiblingIndex(parent.components.Count - 1);
#endif
            }
        }

        public Dictionary<int, Entity> Children
        {
            get
            {
                if (children == null)
                {
                    children = new Dictionary<int, Entity>();
                }

                return this.children;
            }
        }

        public Dictionary<Type, Entity> Components
        {
            get
            {
                if (components == null)
                {
                    components = new Dictionary<Type, Entity>();
                }

                return this.components;
            }
        }

        public T ParentAs<T>() where T : class
        {
            return (object)Parent as T;
        }

        public Entity AddChild(Entity entity)
        {
            if (!entity.IsDisposed)
            {
                throw new Exception($"entity has not disposed!");
            }

            entity.InstanceId = Root.Instance.GenerateInstanceId();
            entity.Parent = this;

            if (entity is IAwake)
            {
                Systems.Awake(entity);
            }

            return entity;
        }

        public T AddChild<T>() where T : Entity, new()
        {
            var entity = new T();
            entity.InstanceId = Root.Instance.GenerateInstanceId();
            entity.Parent = this;

            if (entity is IAwake)
            {
                Systems.Awake(entity);
            }

            return entity;
        }

        public T AddChild<T, A>(A a) where T : Entity, new()
        {
            var entity = new T();
            entity.InstanceId = Root.Instance.GenerateInstanceId();
            entity.Parent = this;

            if (entity is IAwake<A>)
            {
                Systems.Awake(entity, a);
            }

            return entity;
        }

        public T AddChild<T, A, B>(A a, B b) where T : Entity, new()
        {
            var entity = new T();
            entity.InstanceId = Root.Instance.GenerateInstanceId();
            entity.Parent = this;

            if (entity is IAwake<A, B>)
            {
                Systems.Awake(entity, a, b);
            }

            return entity;
        }

        public T AddChild<T, A, B, C>(A a, B b, C c) where T : Entity, new()
        {
            var entity = new T();
            entity.InstanceId = Root.Instance.GenerateInstanceId();
            entity.Parent = this;

            if (entity is IAwake<A, B, C>)
            {
                Systems.Awake(entity, a, b, c);
            }

            return entity;
        }

        public T AddChild<T, A, B, C, D>(A a, B b, C c, D d) where T : Entity, new()
        {
            var entity = new T();
            entity.InstanceId = Root.Instance.GenerateInstanceId();
            entity.Parent = this;

            if (entity is IAwake<A, B, C, D>)
            {
                Systems.Awake(entity, a, b, c, d);
            }

            return entity;
        }

        public Entity GetChild(int instanceId)
        {
            if (children == null)
            {
                return null;
            }

            if (this.children.TryGetValue(instanceId, out var entity))
            {
                return entity;
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

        public void RemoveChild(Entity child)
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

        private void AddToChildren(Entity entity)
        {
            this.Children.Add(entity.m_instanceId, entity);

            if (childrenDB == null)
            {
                childrenDB = new HashSet<Entity>();
            }

            childrenDB.Add(entity);
        }

        private void RemoveFromChildren(Entity entity, int instanceId)
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

            childrenDB.Remove(entity);
        }

        public void AddComponent(Entity component)
        {
            if (!component.IsDisposed)
            {
                throw new Exception($"component has not disposed!");
            }

            var type = component.GetType();
            if (this.components != null && this.components.ContainsKey(type))
            {
                throw new Exception($"entity already has component: {type.FullName}");
            }

            component.InstanceId = Root.Instance.GenerateInstanceId();
            component.ComponentParent = this;

            if (component is IAwake)
            {
                Systems.Awake(component);
            }

            if (this is IAddComponent)
            {
                Systems.AddComponent(this, component);
            }
        }

        public Entity AddComponent(Type type)
        {
            if (this.components != null && this.components.ContainsKey(type))
            {
                throw new Exception($"entity already has component: {type.FullName}");
            }

            var component = Activator.CreateInstance(type) as Entity;
            component.InstanceId = Root.Instance.GenerateInstanceId();
            component.ComponentParent = this;

            if (component is IAwake)
            {
                Systems.Awake(component);
            }

            if (component is IAddComponent)
            {
                Systems.AddComponent(this, component);
            }

            return component;
        }

        public Entity AddComponent<A>(Type type, A a)
        {
            if (this.components != null && this.components.ContainsKey(type))
            {
                throw new Exception($"entity already has component: {type.FullName}");
            }

            var component = Activator.CreateInstance(type) as Entity;
            component.InstanceId = Root.Instance.GenerateInstanceId();
            component.ComponentParent = this;

            if (component is IAwake<A>)
            {
                Systems.Awake(component, a);
            }

            if (this is IAddComponent)
            {
                Systems.AddComponent(this, component);
            }

            return component;
        }

        public Entity AddComponent<A, B>(Type type, A a, B b)
        {
            if (this.components != null && this.components.ContainsKey(type))
            {
                throw new Exception($"entity already has component: {type.FullName}");
            }

            var component = Activator.CreateInstance(type) as Entity;
            component.InstanceId = Root.Instance.GenerateInstanceId();
            component.ComponentParent = this;

            if (component is IAwake<A, B>)
            {
                Systems.Awake(component, a, b);
            }

            if (this is IAddComponent)
            {
                Systems.AddComponent(this, component);
            }

            return component;
        }

        public Entity AddComponent<A, B, C>(Type type, A a, B b, C c)
        {
            if (this.components != null && this.components.ContainsKey(type))
            {
                throw new Exception($"entity already has component: {type.FullName}");
            }

            var component = Activator.CreateInstance(type) as Entity;
            component.InstanceId = Root.Instance.GenerateInstanceId();
            component.ComponentParent = this;

            if (component is IAwake<A, B, C>)
            {
                Systems.Awake(component, a, b, c);
            }

            if (this is IAddComponent)
            {
                Systems.AddComponent(this, component);
            }

            return component;
        }

        public Entity AddComponent<A, B, C, D>(Type type, A a, B b, C c, D d)
        {
            if (this.components != null && this.components.ContainsKey(type))
            {
                throw new Exception($"entity already has component: {type.FullName}");
            }

            var component = Activator.CreateInstance(type) as Entity;
            component.InstanceId = Root.Instance.GenerateInstanceId();
            component.ComponentParent = this;

            if (component is IAwake<A, B, C, D>)
            {
                Systems.Awake(component, a, b, c, d);
            }

            if (this is IAddComponent)
            {
                Systems.AddComponent(this, component);
            }

            return component;
        }

        public T AddComponent<T>() where T : Entity
        {
            return (T)AddComponent(typeof(T));
        }

        public T AddComponent<T, A>(A a) where T : Entity
        {
            return (T)AddComponent(typeof(T), a);
        }

        public T AddComponent<T, A, B>(A a, B b) where T : Entity
        {
            return (T)AddComponent(typeof(T), a, b);
        }

        public T AddComponent<T, A, B, C>(A a, B b, C c) where T : Entity
        {
            return (T)AddComponent(typeof(T), a, b, c);
        }

        public T AddComponent<T, A, B, C, D>(A a, B b, C c, D d) where T : Entity
        {
            return (T)AddComponent(typeof(T), a, b, c, d);
        }

        /// <summary>
        /// 获取组件，如果没有则返回null
        /// </summary>
        /// <param name="deriveMatch"> 是否匹配派生类 </param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>(bool deriveMatch = false)
        {
            return (T)(GetComponent(typeof(T), deriveMatch) as object);
        }

        /// <summary>
        /// 获取组件，如果没有则返回null
        /// </summary>
        /// <param name="type"></param>
        /// <param name="deriveMatch"> 是否匹配派生类 </param>
        /// <returns></returns>
        public Entity GetComponent(Type type, bool deriveMatch = false)
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

        public void RemoveComponent<T>() where T : Entity
        {
            RemoveComponent(typeof(T));
        }

        public void RemoveComponent(Entity component)
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

        private void AddToComponents(Entity component)
        {
            this.Components.Add(component.GetType(), component);

            if (componentsDB == null)
            {
                componentsDB = new HashSet<Entity>();
            }

            componentsDB.Add(component);
        }

        private void RemoveFromComponents(Entity component)
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
            if (this is IDestroy)
            {
                Systems.Destroy(this);
            }

            var instanceId = this.m_instanceId;
            this.InstanceId = 0;

            if (this.children != null)
            {
                foreach (Entity child in this.children.Values)
                {
                    child.Dispose();
                }

                this.children.Clear();
            }

            if (this.components != null)
            {
                foreach (Entity component in this.components.Values)
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