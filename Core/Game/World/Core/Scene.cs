
namespace Atom
{
    public class Scene : Node, IScene
    {
        private string name;

        public override string ViewName => name;

        public Scene(string name, World world)
        {
            this.name = name;
            this.World = world;
            this.Scene = this;
            this.InstanceId = World.GenerateInstanceId();
        }
    }
}