namespace Atom
{
    public interface IGameModule
    {
        void Init();

        void Shutdown();

        void FixedUpdate();

        void Update();

        void LateUpdate();
    }
}