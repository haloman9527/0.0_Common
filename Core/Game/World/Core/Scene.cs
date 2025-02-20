using System;
using System.Collections.Generic;

namespace Moyo
{
    public class Scene : Node
    {
        private Dictionary<string, Scene> scenes;

        public string Name { get; private set; }

        public IEnumerable<Scene> Scenes => scenes == null ? Array.Empty<Scene>() : scenes.Values;

        public new Scene IScene
        {
            get { return base.IScene; }
            set
            {
                if (value == null)
                {
                    throw new Exception($"domain cant set null: {this.GetType().Name}");
                }

                var domainBranch = value.As<Scene>();
                if (domainBranch.scenes != null && domainBranch.scenes.ContainsKey(this.Name))
                {
                    throw new Exception($"domain already exists {this.Name}");
                }

                if (this.IScene != null)
                {
                    var oldDomainBranch = this.IScene.As<Scene>();
                    if (oldDomainBranch != null && oldDomainBranch != domainBranch && oldDomainBranch.scenes.ContainsKey(this.Name))
                    {
                        oldDomainBranch.scenes.Remove(this.Name);
                    }
                }

                if (domainBranch.scenes == null)
                {
                    domainBranch.scenes = new Dictionary<string, Scene>();
                }

                base.IScene = value;
                domainBranch.scenes.Add(this.Name, this);
            }
        }

        public Scene(string name, Node parent, World root)
        {
            Init(name, parent, root);
        }

        internal Scene(string name, World root) : this(name, null, root)
        {
        }

        public Scene(string name, Node parent) : this(name, parent, parent.IScene.World)
        {
        }

        private void Init(string n, Node p, World r)
        {
            this.world = r;
            this.InstanceId = r.GenerateInstanceId();
            this.Name = n;
            if (p == null)
            {
                this.IScene = this;
            }
            else
            {
                var domainBranch = p.IScene.As<Scene>();
                if (domainBranch.scenes != null && domainBranch.scenes.ContainsKey(this.Name))
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

            var oldDomain = this.IScene.As<Scene>();
            base.Dispose();
            scenes?.Clear();
            if (!oldDomain.IsDisposed)
            {
                oldDomain.scenes?.Remove(Name);
            }
        }

        public Scene GetChildScene(string name)
        {
            if (scenes == null)
            {
                return null;
            }

            if (!scenes.TryGetValue(name, out var _branch))
            {
                return null;
            }

            return _branch;
        }
    }

    public static class BranchSystems
    {
        public static Scene AddBranch(this Scene self, string name)
        {
            return new Scene(name, self);
        }
    }
}