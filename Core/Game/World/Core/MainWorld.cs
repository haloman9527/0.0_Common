namespace Atom
{
    public class MainWorld : Singleton<MainWorld>, ISingletonAwake, ISingletonDestory, ISingletonFixedUpdate, ISingletonUpdate, ISingletonLateUpdate
    {
        private World world;

        public World World => world;
        
        public Scene Root => world.Root;

        public void Awake()
        {
            this.world = new World();
        }

        public void Destroy()
        {
            world.Dispose();
        }

        public void FixedUpdate()
        {
            world.Publish<IFixedUpdateSystem>();
        }

        public void Update()
        {
            world.Publish<IUpdateSystem>();
        }

        public void LateUpdate()
        {
            world.Publish<ILateUpdateSystem>();
        }
    }
}