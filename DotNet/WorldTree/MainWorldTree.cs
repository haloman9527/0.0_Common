
namespace CZToolKit
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

        public int GenerateInstanceId()
        {
            return root.GenerateInstanceId();
        }

        public void Add(Node entity)
        {
            root.Add(entity);
        }

        public void Remove(int instanceId)
        {
            root.Remove(instanceId);
        }

        public Node Get(int instanceId)
        {
            return root.Get(instanceId);
        }

        public void FixedUpdate()
        {
            root.FixedUpdate();
        }

        public void Update()
        {
            root.Update();
        }

        public void LateUpdate()
        {
            root.LateUpdate();
        }
    }
}