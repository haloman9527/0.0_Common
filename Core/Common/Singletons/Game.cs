using System;
using System.Collections.Generic;

namespace CZToolKit
{
    public static class Game
    {
        private static readonly Dictionary<Type, ISingleton> singletonTypes = new Dictionary<Type, ISingleton>();
        private static readonly Stack<ISingleton> singletons = new Stack<ISingleton>();
        private static readonly Queue<ISingleton> fixedUpdates = new Queue<ISingleton>();
        private static readonly Queue<ISingleton> updates = new Queue<ISingleton>();
        private static readonly Queue<ISingleton> lateUpdates = new Queue<ISingleton>();

        public static IReadOnlyDictionary<Type, ISingleton> SingleTypes => singletonTypes;

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

            if (singleton is ISingletonFixedUpdate)
                fixedUpdates.Enqueue(singleton);

            if (singleton is ISingletonUpdate)
                updates.Enqueue(singleton);

            if (singleton is ISingletonLateUpdate)
                lateUpdates.Enqueue(singleton);
        }

        public static void FixedUpdate()
        {
            int count = fixedUpdates.Count;
            while (count-- > 0)
            {
                var singleton = fixedUpdates.Dequeue();

                if (singleton.IsDisposed)
                    continue;

                if (!(singleton is ISingletonFixedUpdate fixedUpdate))
                    continue;

                fixedUpdates.Enqueue(singleton);
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

        public static void Update()
        {
            int count = updates.Count;
            while (count-- > 0)
            {
                var singleton = updates.Dequeue();

                if (singleton.IsDisposed)
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

        public static void LateUpdate()
        {
            int count = lateUpdates.Count;
            while (count-- > 0)
            {
                var singleton = lateUpdates.Dequeue();

                if (singleton.IsDisposed)
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

        public static void Close()
        {
            // 顺序反过来清理
            while (singletons.Count > 0)
            {
                var singleton = singletons.Pop();
                if (singleton.IsDisposed)
                    continue;
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