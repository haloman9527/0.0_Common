namespace Atom
{
    public abstract class QueueEventBase
    {
        public abstract bool Completed { get; }

        public abstract void Invoke();

        public abstract void Abort();
    }
}