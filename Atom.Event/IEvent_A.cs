namespace Atom
{
    public interface IEvent<T>
    {
        void Invoke(T arg);
    }
}