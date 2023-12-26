
namespace CZToolKit.ET
{
    public class Scene : Entity
    {
        public string Name { get; }

        public new Entity Domain
        {
            get => this.domain;
            private set => this.domain = value;
        }

        public new Entity Parent
        {
            get { return this.parent; }
            private set
            {
                if (value == null)
                {
                    //this.parent = this;
                    return;
                }

                this.parent = value;
                this.parent.Children.Add(this.InstanceId, this);
            }
        }

        public Scene(string name, Entity parent)
        {
            this.InstanceId = Root.Instance.GenerateInstanceId();
            this.Name = name;
            this.Domain = this;
            this.Parent = parent;
        }
    }
}