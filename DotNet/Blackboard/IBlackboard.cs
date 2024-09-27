namespace CZToolKit.Blackboard
{
    public interface IBlackboard<TKey>
    {
        bool Contains(TKey key);
        
        T Get<T>(TKey key);
        
        object Get(TKey key);

        bool TryGet<T>(TKey key, out T value);
        
        bool TryGet(TKey key, out object value);

        bool Set<T>(TKey key, T value);

        bool Remove(TKey key);

        void Clear();
    }
}