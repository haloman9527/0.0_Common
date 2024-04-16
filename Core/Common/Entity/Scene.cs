using System;
using System.Collections.Generic;

namespace CZToolKit
{
    public sealed class Scene : Entity
    {
        private Dictionary<string, Scene> childScenes;

        public string Name { get; private set; }

        public new Entity Domain
        {
            get { return base.Domain; }
            set
            {
                if (value == null)
                {
                    throw new Exception($"domain cant set null: {this.GetType().Name}");
                }
                
                var domainScene = value.As<Scene>();
                if (domainScene.childScenes != null && domainScene.childScenes.ContainsKey(this.Name))
                {
                    throw new Exception($"domain already exists {this.Name}");
                }

                if (this.Domain != null)
                {
                    var oldDomainScene = this.Domain.As<Scene>();
                    if (oldDomainScene != null && oldDomainScene != domainScene && oldDomainScene.childScenes.ContainsKey(this.Name))
                    {
                        oldDomainScene.childScenes.Remove(this.Name);
                    }
                }
                
                if (domainScene.childScenes == null)
                {
                    domainScene.childScenes = new Dictionary<string, Scene>();
                }

                base.Domain = value;
                domainScene.childScenes.Add(this.Name, this);
            }
        }

        public Scene(string name, Entity parent)
        {
            Init(name, parent);
        }

        private void Init(string name, Entity parent)
        {
            this.InstanceId = Root.Instance.GenerateInstanceId();
            this.Name = name;
            if (parent == null)
                this.Domain = this;
            else
            {
                var domainScene = parent.Domain.As<Scene>();
                if (domainScene.childScenes != null && domainScene.childScenes.ContainsKey(this.Name))
                {
                    throw new Exception($"domain already exists {this.Name}");
                }
                this.Parent = parent;
            }

#if UNITY_EDITOR && ENTITY_PREVIEW
            viewGO.name = name;
#endif
        }

        public override void Dispose()
        {
            var oldDomain = this.Domain.As<Scene>();
            base.Dispose();
            childScenes?.Clear();
            if (!oldDomain.IsDisposed)
            {
                oldDomain.childScenes?.Remove(Name);
            }
        }

        public Scene GetChildScene(string name)
        {
            if (childScenes == null)
            {
                return null;
            }

            if (!childScenes.TryGetValue(name, out var scene))
            {
                return null;
            }

            return scene;
        }
    }

    public static class SceneSystems
    {
        public static Scene AddScene(this Entity self, string name)
        {
            return new Scene(name, self);
        }
    }
}