using CZToolKit.Collections;

namespace CZToolKit.Events
{
    public delegate void EventHandler<T>(T args);

    public class Events
    {
        private MultiDictionary<int, object> eventHandlers;

        public Events()
        {
            eventHandlers = new MultiDictionary<int, object>();
        }

        public void Update()
        {
            
        }

        public void Subscribe<T>(int id, EventHandler<T> handler)
        {
            eventHandlers.Add(id, handler);
        }

        public void UnSubscribe<T>(int id, EventHandler<T> handler)
        {
            eventHandlers.Remove(id, eventHandlers);
        }

        public void FireNow<T>(int id, T args)
        {
            if (eventHandlers.TryGetValue(id, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    ((EventHandler<T>)handler)?.Invoke(args);
                }
            }
        }
    }
}