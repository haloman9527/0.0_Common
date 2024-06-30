using System;
using System.Collections.Generic;

namespace CZToolKit
{
    public class Scene : Node
    {
        private Dictionary<string, Scene> childScenes;

        public string Name { get; private set; }

        public new Scene Domain
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

        private Scene(string name, Node parent, WorldTree root)
        {
            Init(name, parent, root);
        }

        public Scene(string name, Node parent) : this(name, parent, parent.Domain.WorldTree)
        {
        }

        private void Init(string n, Node p, WorldTree r)
        {
            this.WorldTree = r;
            this.InstanceId = r.GenerateInstanceId();
            this.Name = n;
            if (p == null)
            {
                this.Domain = this;
            }
            else
            {
                var domainScene = p.Domain.As<Scene>();
                if (domainScene.childScenes != null && domainScene.childScenes.ContainsKey(this.Name))
                {
                    throw new Exception($"domain already exists {this.Name}");
                }

                this.Parent = p;
            }

#if UNITY_EDITOR && WORLD_TREE_PREVIEW
            viewGO.name = n;
#endif
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

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

        public static Scene NewRootScene(string name, WorldTree root)
        {
            return new Scene(name, null, root);
        }
    }

    public static class SceneSystems
    {
        public static Scene AddScene(this Node self, string name)
        {
            return new Scene(name, self);
        }
    }
}