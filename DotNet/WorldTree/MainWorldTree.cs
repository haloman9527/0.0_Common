
namespace Jiange
{
    public class MainWorldTree : Singleton<MainWorldTree>, ISingletonAwake, ISingletonDestory, ISingletonFixedUpdate, ISingletonUpdate, ISingletonLateUpdate
    {
        private WorldTree root;
        
        public Scene RootScene
        {
            get { return root.Root; }
        }

        public void Awake()
        {
            this.root = new WorldTree();
        }

        public void Destroy()
        {
            root.Dispose();
        }

        public void FixedUpdate()
        {
            root.Publish<IFixedUpdateSystem>();
        }

        public void Update()
        {
            root.Publish<IUpdateSystem>();
        }

        public void LateUpdate()
        {
            root.Publish<ILateUpdateSystem>();
        }
    }
}