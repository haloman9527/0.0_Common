using System;
using System.Collections.Generic;

namespace CZToolKit.Singletons
{
    public static partial class Game
    {
        private static readonly Dictionary<Type, ISingleton> singletonTypes = new Dictionary<Type, ISingleton>();
        private static readonly Stack<ISingleton> singletons = new Stack<ISingleton>();
        private static readonly Queue<ISingleton> updates = new Queue<ISingleton>();
        private static readonly Queue<ISingleton> lateUpdates = new Queue<ISingleton>();

        private static ISingleton GetSingleton_Internal(Type singletonType)
        {
            if (!singletonTypes.TryGetValue(singletonType, out var singleton))
            {
                foreach (var pair in singletonTypes)
                {
                    if (pair.Value.GetType().IsAssignableFrom(singletonType))
                    {
                        singleton = pair.Value;
                        break;
                    }
                }
            }

            return singleton;
        }

        private static void AddSingleton_Internal(ISingleton singleton, Type singletonType)
        {
            singletonTypes.Add(singletonType, singleton);
            singletons.Push(singleton);

            singleton.Register();

            if (singleton is ISingletonAwake awake)
                awake.Awake();

            if (singleton is ISingletonUpdate)
                updates.Enqueue(singleton);

            if (singleton is ISingletonLateUpdate)
                lateUpdates.Enqueue(singleton);
        }

        private static void FixedUpdateSingletons()
        {
            int count = updates.Count;
            while (count-- > 0)
            {
                ISingleton singleton = updates.Dequeue();

                if (singleton.IsDisposed())
                    continue;

                if (!(singleton is ISingletonFixedUpdate fixedUpdate))
                    continue;

                updates.Enqueue(singleton);
                try
                {
                    fixedUpdate.FixedUpdate();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        private static void UpdateSingletons()
        {
            int count = updates.Count;
            while (count-- > 0)
            {
                ISingleton singleton = updates.Dequeue();

                if (singleton.IsDisposed())
                    continue;

                if (!(singleton is ISingletonUpdate update))
                    continue;

                updates.Enqueue(singleton);
                try
                {
                    update.Update();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        private static void LateUpdateSingletons()
        {
            int count = lateUpdates.Count;
            while (count-- > 0)
            {
                ISingleton singleton = lateUpdates.Dequeue();

                if (singleton.IsDisposed())
                    continue;

                if (!(singleton is ISingletonLateUpdate lateUpdate))
                    continue;

                lateUpdates.Enqueue(singleton);
                try
                {
                    lateUpdate.LateUpdate();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        private static void CloseSingletons()
        {
            // 顺序反过来清理
            while (singletons.Count > 0)
            {
                ISingleton singleton = singletons.Pop();
                singleton.Dispose();
            }

            singletonTypes.Clear();
        }

        public static bool IsInitialized(Type singletonType)
        {
            return singletonTypes.ContainsKey(singletonType);
        }

        public static ISingleton GetSingleton(Type singletonType)
        {
            return GetSingleton_Internal(singletonType);
        }

        public static T GetSingleton<T>()
        {
            return (T)GetSingleton_Internal(typeof(T));
        }

        public static T AddSingleton<T>() where T : Singleton<T>, new()
        {
            T singleton = new T();
            AddSingleton_Internal(singleton, typeof(T));
            return singleton;
        }

        public static void AddSingleton(ISingleton singleton)
        {
            Type singletonType = singleton.GetType();
            AddSingleton_Internal(singleton, singletonType);
        }
    }
}