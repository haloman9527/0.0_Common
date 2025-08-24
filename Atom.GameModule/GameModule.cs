namespace Atom
{
    public abstract class GameModule : IGameModule
    {
        public virtual void Init()
        {
        }

        public virtual void Shutdown()
        {
        }
        
        public virtual void FixedUpdate()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void LateUpdate()
        {
        }
    }
}