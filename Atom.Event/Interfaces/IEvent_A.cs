namespace Atom
{
    public interface IEvent<T>
    {
        void Invoke(in T arg);
    }
}