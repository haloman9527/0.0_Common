using System;
using System.Collections.Generic;

namespace CZToolKit
{
    public sealed class Scene : Node
    {
        private Dictionary<string, Scene> childScenes;

        public string Name { get; private set; }

        public new Node Parent
        {
            get { return base.Parent; }
            private set { throw new Exception("domain cannot change parent"); }
        }

        public new Node Domain
        {
            get { return base.Domain; }
            private set
            {
                if (value == null)
                {
                    throw new Exception($"domain cant set null: {this.GetType().Name}");
                }

                this.domain = value;
            }
        }

        public Scene(string name, Node parent)
        {
            Init(name, parent);
        }

        private void Init(string name, Node parent)
        {
            this.InstanceId = Root.Instance.GenerateInstanceId();
            this.Name = name;
            if (parent == null)
                this.Domain = this;
            else
                base.Parent = parent;

            if (this.domain != this)
            {
                if (domain.As<Scene>().childScenes == null)
                {
                    domain.As<Scene>().childScenes = new Dictionary<string, Scene>();
                }

                domain.As<Scene>().childScenes.Add(name, this);
            }

#if UNITY_EDITOR && NODE_PREVIEW
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
        public static Scene AddScene(this Node self, string name)
        {
            return new Scene(name, self);
        }
    }
}