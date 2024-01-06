
namespace CZToolKit
{
    public interface IEvent
    {
    }

    public interface IEvent<A> : IEvent where A : struct
    {
        void Handle(A arg);
    }
}