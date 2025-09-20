namespace Atom
{
    public abstract class GameModuleSingleton<TGameModule> : IGameModule where TGameModule : GameModuleSingleton<TGameModule>
    {
        private static TGameModule s_Instance;

        public static TGameModule Instance => s_Instance;

        public void Init()
        {
            if (s_Instance == null)
                s_Instance = this as TGameModule;

            this.OnInit();
        }

        public void UnInit()
        {
            if (s_Instance == this)
                s_Instance = null;

            this.OnUnInit();
        }

        public void FixedUpdate()
        {
            this.OnFixedUpdate();
        }

        public void Update()
        {
            this.OnUpdate();
        }

        public void LateUpdate()
        {
            this.OnLateUpdate();
        }

        protected virtual void OnInit()
        {
        }

        protected virtual void OnUnInit()
        {
        }

        protected virtual void OnFixedUpdate()
        {
        }

        protected virtual void OnUpdate()
        {
        }

        protected virtual void OnLateUpdate()
        {
        }
    }
}