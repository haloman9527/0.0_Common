using System;
using System.Collections.Generic;

namespace CZToolKit.ET
{
    [Serializable]
    public class Entity : IDisposable
    {
#if UNITY_EDITOR
        protected UnityEngine.GameObject viewGO;
#endif

        private int instanceId;
        protected Entity domain;
        protected Entity parent;
        private Dictionary<int, Entity> children = new Dictionary<int, Entity>();
        private Dictionary<Type, Entity> components = new Dictionary<Type, Entity>();

        public int InstanceId
        {
            get { return instanceId; }
            protected set
            {
                if (this.instanceId == value)
                {
                    return;
                }

                if (value == 0)
                {
                    Root.Instance.Remove(this.instanceId);
                    instanceId = value;
                }
                else
                {
                    instanceId = value;
                    Root.Instance.Add(this);
                }

#if UNITY_EDITOR
                if (instanceId != 0)
                {
                    this.viewGO = new UnityEngine.GameObject(this.GetType().Name);
                    this.viewGO.AddComponent<EntityPreview>().Component = this;
                    this.viewGO.transform.SetParent(this.Parent == null ? EntityPreviewRoot.Instance.transform : this.Parent.viewGO.transform);
                }
                else
                {
                    UnityEngine.GameObject.Destroy(viewGO);
                }
#endif
            }
        }

        public bool IsDisposed
        {
            get { return instanceId == 0; }
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
                    this.parent.RemoveFromChildren(this);
                }

                this.parent = value;
                this.parent.AddToChildren(this);
                this.Domain = parent.Domain;


#if UNITY_EDITOR
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

                    this.parent.RemoveFromChildren(this);
                }

                this.parent = value;
                this.parent.AddToComponents(this);
                this.domain = this.parent.domain;
                
#if UNITY_EDITOR
                if (parent.viewGO.transform.Find("---------------") == null)
                {
                    new UnityEngine.GameObject("---------------").transform.SetParent(parent.viewGO.transform, false);
                }

                this.viewGO.transform.SetParent(parent.viewGO.transform, false);
                this.viewGO.transform.SetAsFirstSibling();
#endif
            }
        }

        public Dictionary<int, Entity> Children
        {
            get { return this.children; }
        }

        public Dictionary<Type, Entity> Components
        {
            get { return this.components; }
        }

        public Entity AddChild(Entity entity)
        {
            entity.Parent = this;
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

        public Entity GetChild(int instanceId)
        {
            if (this.children.TryGetValue(instanceId, out var entity))
                return entity;
            return null;
        }

        public void RemoveChild(int instanceId)
        {
            if (this.IsDisposed)
            {
                return;
            }

            var entity = this.GetChild(instanceId);
            if (entity != null)
            {
                entity.Dispose();
            }
        }

        private void AddToChildren(Entity entity)
        {
            this.children.Add(entity.instanceId, entity);
        }

        public void RemoveFromChildren(Entity entity)
        {
            this.children.Remove(entity.instanceId);
        }

        public Entity AddComponent(Entity component)
        {
            var type = component.GetType();
            if (this.components != null && this.components.ContainsKey(type))
            {
                throw new Exception($"entity already has component: {type.FullName}");
            }

            component.InstanceId = Root.Instance.GenerateInstanceId();
            component.ComponentParent = this;

            if (this is IAddComponent)
            {
                Systems.AddComponent(this, component);
            }

            return component;
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

            if (this is IAwake)
            {
                Systems.Awake(component);
            }

            if (this is IAddComponent)
            {
                Systems.AddComponent(this, component);
            }

            return component;
        }

        public T AddComponent<T>() where T : Entity, new()
        {
            return (T)AddComponent(typeof(T));
        }

        public T GetComponent<T>()
        {
            var type = typeof(T);
            if (!this.components.TryGetValue(type, out var component))
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
            return (T)(component as object);
        }
            
        public Entity GetComponent(Type type)
        {
            if (!this.components.TryGetValue(type, out var component))
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

            Entity c = this.GetComponent(type);
            if (c == null)
            {
                return;
            }

            c.Dispose();
        }

        public void RemoveComponent(Entity component)
        {
            if (this.IsDisposed)
            {
                return;
            }

            Entity c = this.GetComponent(component.GetType());
            if (c == null)
            {
                return;
            }

            if (c.instanceId != component.instanceId)
            {
                return;
            }

            c.Dispose();
        }

        private void AddToComponents(Entity component)
        {
            this.components.Add(component.GetType(), component);
        }

        public void RemoveFromComponents(Entity component)
        {
            this.components.Remove(component.GetType());
        }

        public virtual void Dispose()
        {
            if (this is IDestroy)
            {
                Systems.Destroy(this);
            }

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

            if (this.parent != null && this.parent.IsDisposed == false)
            {
                this.parent.RemoveComponent(this);
                this.parent.RemoveFromChildren(this);
            }

            this.parent = null;
            this.domain = null;
        }
    }
}