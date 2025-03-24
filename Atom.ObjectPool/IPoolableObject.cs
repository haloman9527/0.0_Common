namespace Atom
{
    public interface IPoolableObject
    {
        void OnSpawn();

        void OnRecycle();
    }
}