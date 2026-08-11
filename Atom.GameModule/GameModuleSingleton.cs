namespace Atom
{
    public abstract class GameModuleSingleton<TGameModule> : IGameModule where TGameModule : GameModuleSingleton<TGameModule>
    {
        private static TGameModule s_Instance;

        public static TGameModule Instance => s_Instance;

        void IGameModule.Init()
        {
            if (s_Instance == null)
                s_Instance = this as TGameModule;

            this.Init();
        }

        void IGameModule.UnInit()
        {
            if (s_Instance == this)
                s_Instance = null;

            this.UnInit();
        }

        void IGameModule.FixedUpdate()
        {
            this.FixedUpdate();
        }

        void IGameModule.Update()
        {
            this.Update();
        }

        void IGameModule.LateUpdate()
        {
            this.LateUpdate();
        }

        protected virtual void Init()
        {
        }

        protected virtual void UnInit()
        {
        }

        protected virtual void FixedUpdate()
        {
        }

        protected virtual void Update()
        {
        }

        protected virtual void LateUpdate()
        {
        }
    }
}