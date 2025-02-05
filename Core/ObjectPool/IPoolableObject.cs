namespace Moyo
{
    public interface IPoolableObject
    {
        void OnSpawn();

        void OnRecycle();
    }
}