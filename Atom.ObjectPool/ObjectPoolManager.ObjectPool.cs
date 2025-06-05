
namespace Atom
{
    public static partial class ObjectPoolManager
    {
        private class ObjectPool<T> : ObjectPoolBase<T> where T : class, new()
        {
            protected override T Create()
            {
                return new T();
            }
        }
    }
}