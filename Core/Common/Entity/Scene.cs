
using System;

namespace CZToolKit.ET
{
    public class Scene : Entity
    {
        private string name;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                
#if UNITY_EDITOR
                viewGO.name = name;
#endif
            }
        }

        public new Entity Parent
        {
            get { return base.Parent; }
            set
            {
                throw new Exception("Scene cannot set parent");
            }
        }

        public Scene(string name, Entity parent)
        {
            this.InstanceId = Root.Instance.GenerateInstanceId();
            this.Name = name;
            if (parent == null)
            {
                base.Domain = this;
            }
            else
            {
                base.Parent = parent;
            }
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